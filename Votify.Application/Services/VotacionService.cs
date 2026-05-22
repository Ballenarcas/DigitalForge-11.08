using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Application.Services.Estrategia;
using Votify.Domain.Entities;
using Votify.Domain.Factory;
using Votify.Domain.Interfaces;
using Votify.Domain.Estado;
using EstadoActiva = Votify.Domain.Estado.EstadoActiva;
using EstadoPausada = Votify.Domain.Estado.EstadoPausada;
using EstadoFinalizada = Votify.Domain.Estado.EstadoFinalizada;

namespace Votify.Application.Services
{
    public class VotacionService : IVotacionService
    {
        private readonly IVotacionRepository _repo;
        private readonly IVotoRepository _votoRepo;
        private readonly IProyectoRepository _proyectoRepo;
        private readonly IEventoRepository _eventoRepo;
        private readonly ICriterioRepository? _criterioRepo;
        private readonly IValoracionCriterioRepository? _valoracionCriterioRepo;
        private readonly IEquipoRepository _equipoRepo;
        private readonly VotacionStrategyResolver _strategyResolver;
        private readonly IVotacionObservable _votacionObservable;
        private readonly IManualVotosService _manualVotosService;

        public VotacionService(
            IVotacionRepository repo,
            IVotoRepository votoRepo,
            IProyectoRepository proyectoRepo,
            IEventoRepository eventoRepo,
            IEquipoRepository equipoRepo,
            VotacionStrategyResolver strategyResolver,
            IVotacionObservable votacionObservable,
            IManualVotosService manualVotosService,
            ICriterioRepository? criterioRepo = null,
            IValoracionCriterioRepository? valoracionCriterioRepo = null)
        {
            _repo = repo;
            _votoRepo = votoRepo;
            _proyectoRepo = proyectoRepo;
            _eventoRepo = eventoRepo;
            _equipoRepo = equipoRepo;
            _strategyResolver = strategyResolver;
            _votacionObservable = votacionObservable;
            _manualVotosService = manualVotosService;
            _criterioRepo = criterioRepo;
            _valoracionCriterioRepo = valoracionCriterioRepo;
        }

        public async Task CrearVotacionAsync(CrearVotacionDto dto)
        {
            await ValidarFechasContraEventoAsync(dto);
            ValidarCriterios(dto);
            var votacion = CreateEntityFromDto(dto);
            await _repo.GuardarAsync(votacion);
            await _votacionObservable.NotificarVotacionCreadaAsync(votacion);
            if (EsMulticriterio(dto.Tipo) && _criterioRepo is not null)
            {
                await _criterioRepo.ReemplazarPorVotacionAsync(votacion.Id.ToString(), MapCriterios(dto.Criterios));
            }
        }
        public async Task<List<CrearVotacionResponse>> ObtenerTodasAsync()
        {
            var entidades = await _repo.ObtenerTodasAsync();
            // Actualizar estados automáticamente según fechas
            await ActualizarEstadosAutomaticosAsync(entidades);
            var responses = new List<CrearVotacionResponse>();
            foreach (var entidad in entidades)
            {
                responses.Add(await MapToResponseAsync(entidad));
            }
            return responses;
        }
        public async Task<List<CrearVotacionResponse>> ObtenerPorEventoAsync(string eventoId)
        {
            if (!Guid.TryParse(eventoId, out var guid)) return new List<CrearVotacionResponse>();
            
            var entidades = await _repo.ObtenerPorEventoAsync(guid);
            // Actualizar estados automáticamente según fechas
            await ActualizarEstadosAutomaticosAsync(entidades);
            var responses = new List<CrearVotacionResponse>();
            foreach (var entidad in entidades)
            {
                responses.Add(await MapToResponseAsync(entidad));
            }
            return responses;
        }

        public async Task<CrearVotacionResponse?> ObtenerPorIdAsync(string id)
        {
            var e = await _repo.ObtenerAsync(id);
            if (e is null) return null;

            // Actualizar estado automáticamente según fechas SOLO si nunca fue pausada manualmente
            // Si está pausada, respetamos la decisión del usuario
            if (e.Estado is not EstadoPausada)
            {
                var estadoAnterior = e.Estado;
                ActualizarEstadoAutomatico(e);
                if (!ReferenceEquals(e.Estado, estadoAnterior))
                {
                    await _repo.ActualizarAsync(id, e);
                }
            }
            
            return await MapToResponseAsync(e);
        }
        public async Task ActualizarVotacionAsync(string id, CrearVotacionDto dto)
        {
            await ValidarFechasContraEventoAsync(dto);
            ValidarCriterios(dto);
            var votacion = CreateEntityFromDto(dto);
            votacion.Id = Guid.Parse(id);

            if (dto.Estado.HasValue)
            {
                var targetState = (EstadoVotacion)dto.Estado.Value;
                switch (targetState)
                {
                    case EstadoVotacion.Abierta:
                        votacion.Abrir();
                        break;
                    case EstadoVotacion.Pausada:
                        votacion.Pausar();
                        break;
                    case EstadoVotacion.Detenida:
                        votacion.Detener();
                        break;
                }
            }

            var actualizado = await _repo.ActualizarAsync(id, votacion);
            if (!actualizado)
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");

            if (_criterioRepo is not null)
            {
                if (EsMulticriterio(dto.Tipo))
                {
                    await _criterioRepo.ReemplazarPorVotacionAsync(id, MapCriterios(dto.Criterios));
                }
                else
                {
                    await _criterioRepo.EliminarPorVotacionAsync(id);
                }
            }
        }

        public async Task EliminarVotacionAsync(string id)
        {

            if (_valoracionCriterioRepo is not null)
            {
                await _valoracionCriterioRepo.EliminarPorVotacionAsync(id);
            }
            if (_criterioRepo is not null)
            {
                await _criterioRepo.EliminarPorVotacionAsync(id);
            }
            var votosEliminados = await _votoRepo.EliminarPorVotacionAsync(id);
            

            var eliminado = await _repo.EliminarAsync(id);
            if (!eliminado)
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");
        }

        public async Task<List<ResultadoProyectoDto>> ObtenerResultadosAsync(string votacionId)
        {
            var votacion = await _repo.ObtenerAsync(votacionId);
            if (votacion is null)
                return new List<ResultadoProyectoDto>();

            var strategy = _strategyResolver.Resolver(votacion.Tipo);
            var resultados = await strategy.CalcularResultadosAsync(votacionId);

            var asignacionesManuales = await _manualVotosService.ObtenerAsignacionesManualesAsync(votacionId);
            var manualDict = asignacionesManuales.ToDictionary(m => m.Id, m => m);

            var fusionados = new List<ResultadoProyectoDto>();

            foreach (var resultado in resultados)
            {
                if (manualDict.TryGetValue(resultado.Id, out var manual))
                {
                    fusionados.Add(new ResultadoProyectoDto
                    {
                        Id = resultado.Id,
                        Nombre = resultado.Nombre,
                        Equipo = resultado.Equipo,
                        TotalVotos = manual.TotalVotos,
                        Posicion = manual.Posicion,
                        IsManual = true,
                        Justificacion = manual.Justificacion
                    });
                }
                else
                {
                    fusionados.Add(resultado);
                }
            }

            foreach (var manual in asignacionesManuales.Where(m => !fusionados.Any(f => f.Id == m.Id)))
            {
                fusionados.Add(manual);
            }

            return fusionados.OrderBy(r => r.Posicion).ToList();
        }

        public async Task<List<ResultadoMulticriterioDto>> ObtenerResultadosMulticriterioAsync(string votacionId)
        {
            var votacion = await _repo.ObtenerAsync(votacionId);
            if (votacion is null)
                return new List<ResultadoMulticriterioDto>();

            var strategy = _strategyResolver.Resolver(votacion.Tipo);
            return await strategy.CalcularResultadosMulticriterioAsync(votacionId);
        }

        public async Task PausarVotacionAsync(string id)
        {
            var votacion = await _repo.ObtenerAsync(id);
            if (votacion is null)
            {
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");
            }

            votacion.Pausar();
            var actualizado = await _repo.ActualizarAsync(id, votacion);
            if (!actualizado)
            {
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");
            }
            await _votacionObservable.NotificarVotacionPausadaAsync(votacion);
        }

        public async Task DetenerVotacionAsync(string id)
        {
            var votacion = await _repo.ObtenerAsync(id);
            if (votacion is null)
            {
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");
            }

            votacion.Detener();
            var actualizado = await _repo.ActualizarAsync(id, votacion);
            if (!actualizado)
            {
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");
            }
            await _votacionObservable.NotificarVotacionDetenidaAsync(votacion);
        }

        public async Task AbrirVotacionAsync(string id)
        {
            var votacion = await _repo.ObtenerAsync(id);
            if (votacion is null)
            {
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");
            }

            // Validar que el período de votación siga siendo válido
            if (DateTime.UtcNow > votacion.FechaFin)
            {
                throw new InvalidOperationException("No se puede reanudar una votación cuyo período ha finalizado.");
            }

            votacion.Abrir();
            var actualizado = await _repo.ActualizarAsync(id, votacion);
            if (!actualizado)
            {
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");
            }
            await _votacionObservable.NotificarVotacionAbiertaAsync(votacion);
        }

        private CrearVotacionResponse MapToResponse(Votacion e, List<CriterioDto>? criterios = null)
        {
            return new CrearVotacionResponse
            {
                Id = e.Id.ToString(),
                Nombre = e.Nombre,
                Tipo = e.Tipo,
                FechaInicio = e.FechaInicio,
                FechaFin = e.FechaFin,
                LimiteProy = e.LimiteProy,
                Comentarios = e.Comentarios,
                ComentariosObligatorios = e.ComentariosObligatorios,
                EsAnonima = e.EsAnonima,
                EventoId = e.EventoId.ToString(),
                Estado = e.Estado is EstadoActiva ? 0 : e.Estado is EstadoPausada ? 1 : 2,
                ImagenUrl = e.ImagenUrl,
                Criterios = criterios ?? new List<CriterioDto>()
            };
        }

        private async Task<CrearVotacionResponse> MapToResponseAsync(Votacion e)
        {
            var criterios = _criterioRepo is null
                ? new List<CriterioDto>()
                : (await _criterioRepo.ObtenerPorVotacionAsync(e.Id.ToString())).Select(MapCriterioDto).ToList();

            return MapToResponse(e, criterios);
        }

        private void ActualizarEstadoAutomatico(Votacion votacion)
        {
            var ahora = DateTime.UtcNow;
            
            // Si aún no ha llegado la fecha de inicio, la votación debería estar pausada
            if (ahora < votacion.FechaInicio && votacion.Estado is EstadoActiva)
            {
                votacion.Pausar();
            }
            else if (ahora >= votacion.FechaFin && votacion.Estado is EstadoActiva)
            {
                votacion.Detener();
            }
        }




        private async Task ActualizarEstadosAutomaticosAsync(List<Votacion> votaciones)
        {
            var votacionesActualizadas = new List<(Votacion votacion, bool cambio)>();

            foreach (var votacion in votaciones)
            {
                if (votacion.Estado is EstadoPausada)
                    continue;

                var estadoAnterior = votacion.Estado;
                ActualizarEstadoAutomatico(votacion);

                if (!ReferenceEquals(votacion.Estado, estadoAnterior))
                {
                    votacionesActualizadas.Add((votacion, true));
                }
            }

            foreach (var (votacion, _) in votacionesActualizadas)
            {
                await _repo.ActualizarAsync(votacion.Id.ToString(), votacion);

                if (votacion.Estado is EstadoPausada)
                    await _votacionObservable.NotificarVotacionPausadaAsync(votacion);
                else if (votacion.Estado is EstadoFinalizada)
                    await _votacionObservable.NotificarVotacionDetenidaAsync(votacion);
            }
        }

        private Votacion CreateEntityFromDto(CrearVotacionDto dto)
        {
            if (dto.FechaInicio >= dto.FechaFin)
            {
                throw new ArgumentException("La fecha de inicio debe ser menor a la fecha de fin.");
            }

            if (!Guid.TryParse(dto.EventoId, out var eventoGuid))
            {
                throw new ArgumentException("El ID del evento no es válido o no se ha proporcionado.");
            }

            var tipo = dto.Tipo.ToUpper();
            Votacion votacion = tipo switch
            {
                "ESTANDAR" => new VotacionEstandar(
                    dto.Nombre,
                    dto.FechaInicio,
                    dto.FechaFin,
                    dto.LimiteProy,
                    dto.Comentarios,
                    dto.ComentariosObligatorios,
                    eventoGuid,
                    dto.EsAnonima,
                    dto.ImagenUrl),

                "MULTICRITERIO" => new VotacionMulticriterio(
                    dto.Nombre,
                    dto.FechaInicio,
                    dto.FechaFin,
                    dto.LimiteProy,
                    dto.Comentarios,
                    dto.ComentariosObligatorios,
                    eventoGuid,
                    dto.EsAnonima,
                    dto.ImagenUrl),

                "MULTICRITERIO_PUBLICO" => new VotacionMulticriterioPublico(
                    dto.Nombre,
                    dto.FechaInicio,
                    dto.FechaFin,
                    dto.LimiteProy,
                    dto.Comentarios,
                    dto.ComentariosObligatorios,
                    eventoGuid,
                    dto.EsAnonima,
                    dto.ImagenUrl),

                _ => throw new ArgumentException($"Tipo de votación no válido: {tipo}. Use 'ESTANDAR', 'MULTICRITERIO' o 'MULTICRITERIO_PUBLICO'.")
            };

            if (dto.FechaInicio > DateTime.UtcNow)
            {
                votacion.Pausar();
            }

            return votacion;
        }
        private async Task ValidarFechasContraEventoAsync(CrearVotacionDto dto)
        {
            if (!Guid.TryParse(dto.EventoId, out _)) return; // se valida luego en CreateEntityFromDto

            var evento = await _eventoRepo.ObtenerPorIdAsync(dto.EventoId);
            if (evento is null) return; // si no existe el evento, que falle en FK

            if (dto.FechaInicio < evento.FechaInicio || dto.FechaFin > evento.FechaFin)
            {
                throw new ArgumentException(
                    $"Las fechas de la votación deben estar dentro del evento " +
                    $"({evento.FechaInicio:dd/MM/yyyy HH:mm} – {evento.FechaFin:dd/MM/yyyy HH:mm}).");
            }
        }

        private void ValidarCriterios(CrearVotacionDto dto)
        {
            if (!EsMulticriterio(dto.Tipo))
            {
                return;
            }

            if (dto.Criterios.Count == 0)
            {
                throw new ArgumentException("La votación multicriterio debe tener al menos un criterio.");
            }

            if (dto.Criterios.Any(c => string.IsNullOrWhiteSpace(c.Nombre) || c.Peso <= 0))
            {
                throw new ArgumentException("Todos los criterios deben tener nombre y peso mayor que cero.");
            }

            var total = dto.Criterios.Sum(c => c.Peso);
            if (Math.Abs(total - 100m) > 0.01m)
            {
                throw new ArgumentException("Los pesos de los criterios deben sumar 100%.");
            }
        }

        private static bool EsMulticriterio(string? tipo)
        {
            var t = tipo?.Trim().ToUpper();
            return t == "MULTICRITERIO" || t == "MULTICRITERIO_PUBLICO";
        }

        private static List<Criterio> MapCriterios(List<CriterioDto> criterios)
        {
            return criterios.Select(c => new Criterio
            {
                Id = Guid.TryParse(c.Id, out var id) ? id : Guid.NewGuid(),
                Nombre = c.Nombre.Trim(),
                Tipo = string.IsNullOrWhiteSpace(c.Tipo) ? "Estrellas" : c.Tipo,
                Peso = c.Peso
            }).ToList();
        }

        private static CriterioDto MapCriterioDto(Criterio criterio)
        {
            return new CriterioDto
            {
                Id = criterio.Id.ToString(),
                Nombre = criterio.Nombre,
                Tipo = criterio.Tipo,
                Peso = criterio.Peso
            };
        }
    }
}
