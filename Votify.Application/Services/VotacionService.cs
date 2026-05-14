using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;
using Votify.Domain.Factory;
using Votify.Domain.Interfaces;

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

        public VotacionService(
            IVotacionRepository repo,
            IVotoRepository votoRepo,
            IProyectoRepository proyectoRepo,
            IEventoRepository eventoRepo,
            IEquipoRepository equipoRepo,
            ICriterioRepository? criterioRepo = null,
            IValoracionCriterioRepository? valoracionCriterioRepo = null)
        {
            _repo = repo;
            _votoRepo = votoRepo;
            _proyectoRepo = proyectoRepo;
            _eventoRepo = eventoRepo;
            _equipoRepo = equipoRepo;
            _criterioRepo = criterioRepo;
            _valoracionCriterioRepo = valoracionCriterioRepo;
        }

        public async Task CrearVotacionAsync(CrearVotacionDto dto)
        {
            await ValidarFechasContraEventoAsync(dto);
            ValidarCriterios(dto);
            var votacion = CreateEntityFromDto(dto);
            await _repo.GuardarAsync(votacion);
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
            if (e.Estado != EstadoVotacion.Pausada)
            {
                var estadoAnterior = e.Estado;
                ActualizarEstadoAutomatico(e);
                if (e.Estado != estadoAnterior)
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

            // Actualizar Estado si se proporciona
            if (dto.Estado.HasValue)
            {
                votacion.Estado = (EstadoVotacion)dto.Estado.Value;
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
            var equipos = await _equipoRepo.ObtenerTodosAsync();
            var equipoDict = equipos.ToDictionary(e => e.Id.ToString(), e => e.Nombre);

            if (votacion is not null && EsMulticriterio(votacion.Tipo) && _valoracionCriterioRepo is not null)
            {
                var ponderados = await _valoracionCriterioRepo.ObtenerResultadosPonderadosAsync(votacionId);
                var proyectosMulti = await _proyectoRepo.ObtenerPorVotacionAsync(votacionId);
                var proyectoMultiDict = proyectosMulti.ToDictionary(p => p.Id);

                return ponderados.Select((resultado, index) => new ResultadoProyectoDto
                {
                    Id = resultado.ProyectoId,
                    Nombre = proyectoMultiDict.ContainsKey(resultado.ProyectoId) ? proyectoMultiDict[resultado.ProyectoId].Nombre : "Proyecto desconocido",
                    Equipo = proyectoMultiDict.ContainsKey(resultado.ProyectoId) && proyectoMultiDict[resultado.ProyectoId].Equipo_Id != null && equipoDict.ContainsKey(proyectoMultiDict[resultado.ProyectoId].Equipo_Id!) 
                        ? equipoDict[proyectoMultiDict[resultado.ProyectoId].Equipo_Id!] 
                        : "Sin equipo",
                    TotalVotos = resultado.Evaluaciones,
                    PuntajeFinal = Math.Round(resultado.Puntaje, 2),
                    Evaluaciones = resultado.Evaluaciones,
                    Posicion = index + 1
                }).ToList();
            }

            var votosporProyecto = await _votoRepo.ObtenerVotosPorVotacionAsync(votacionId);

            if (votosporProyecto.Count == 0)
                return new List<ResultadoProyectoDto>();

            var proyectos = await _proyectoRepo.ObtenerPorVotacionAsync(votacionId);
            var proyectoDict = proyectos.ToDictionary(p => p.Id);

            var resultados = votosporProyecto
                .Select((vp, index) => new ResultadoProyectoDto
                {
                    Id = vp.ProyectoId,
                    Nombre = proyectoDict.ContainsKey(vp.ProyectoId) ? proyectoDict[vp.ProyectoId].Nombre : "Proyecto desconocido",
                    Equipo = proyectoDict.ContainsKey(vp.ProyectoId) && proyectoDict[vp.ProyectoId].Equipo_Id != null && equipoDict.ContainsKey(proyectoDict[vp.ProyectoId].Equipo_Id!) 
                        ? equipoDict[proyectoDict[vp.ProyectoId].Equipo_Id!] 
                        : "Sin equipo",
                    TotalVotos = vp.Votos,
                    Posicion = index + 1
                })
                .ToList();

            return resultados;
        }

        public async Task<List<ResultadoMulticriterioDto>> ObtenerResultadosMulticriterioAsync(string votacionId)
        {
            var votacion = await _repo.ObtenerAsync(votacionId);
            if (votacion is null || !EsMulticriterio(votacion.Tipo))
                return new List<ResultadoMulticriterioDto>();

            if (_valoracionCriterioRepo is null || _criterioRepo is null)
                return new List<ResultadoMulticriterioDto>();

            var equipos = await _equipoRepo.ObtenerTodosAsync();
            var equipoDict = equipos.ToDictionary(e => e.Id.ToString(), e => e.Nombre);

            // Get overall ponderados
            var ponderados = await _valoracionCriterioRepo.ObtenerResultadosPonderadosAsync(votacionId);
            // Get per-criterion details
            var detalles = await _valoracionCriterioRepo.ObtenerDetallesPorCriterioAsync(votacionId);
            // Get criterios (names/weights)
            var criterios = await _criterioRepo.ObtenerPorVotacionAsync(votacionId);
            var criterioDict = criterios.ToDictionary(c => c.Id.ToString(), c => c);
            // Get projects
            var proyectos = await _proyectoRepo.ObtenerPorVotacionAsync(votacionId);
            var proyectoDict = proyectos.ToDictionary(p => p.Id);

            return ponderados.Select((resultado, index) =>
            {
                var detallesProyecto = detalles
                    .Where(d => d.ProyectoId == resultado.ProyectoId)
                    .Select(d =>
                    {
                        var criterio = criterioDict.ContainsKey(d.CriterioId.ToString())
                            ? criterioDict[d.CriterioId.ToString()]
                            : null;
                        return new DetalleCriterioResultadoDto
                        {
                            CriterioId = d.CriterioId.ToString(),
                            CriterioNombre = criterio?.Nombre ?? "Criterio desconocido",
                            Peso = criterio?.Peso ?? 0,
                            PromedioValoracion = Math.Round(d.PromedioValoracion, 2),
                            PuntajePonderado = Math.Round(d.PromedioValoracion * (double)(criterio?.Peso ?? 0) / 100.0, 2)
                        };
                    })
                    .OrderByDescending(d => d.PuntajePonderado)
                    .ToList();

                return new ResultadoMulticriterioDto
                {
                    Id = resultado.ProyectoId,
                    Nombre = proyectoDict.ContainsKey(resultado.ProyectoId) ? proyectoDict[resultado.ProyectoId].Nombre : "Proyecto desconocido",
                    Equipo = proyectoDict.ContainsKey(resultado.ProyectoId) && proyectoDict[resultado.ProyectoId].Equipo_Id != null && equipoDict.ContainsKey(proyectoDict[resultado.ProyectoId].Equipo_Id!) 
                        ? equipoDict[proyectoDict[resultado.ProyectoId].Equipo_Id!] 
                        : "Sin equipo",
                    PuntajeFinal = Math.Round(resultado.Puntaje, 2),
                    Evaluaciones = resultado.Evaluaciones,
                    Posicion = index + 1,
                    DetallesCriterios = detallesProyecto
                };
            }).ToList();
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
                Estado = (int)e.Estado,
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
            if (ahora < votacion.FechaInicio && votacion.Estado == EstadoVotacion.Abierta)
            {
                votacion.Pausar();
            }
            // NO reabrimos automáticamente votaciones pausadas manualmente
            // La pausa es una acción deliberada del usuario que debe respetarse
            // Si ya pasó la fecha final, solo detener la votación si seguía activa
            // IMPORTANTE: No finalizamos votaciones pausadas - respetamos el estado manual
            else if (ahora >= votacion.FechaFin && votacion.Estado == EstadoVotacion.Abierta)
            {
                votacion.Detener();
            }
        }




        private async Task ActualizarEstadosAutomaticosAsync(List<Votacion> votaciones)
        {
            var votacionesActualizadas = new List<(Votacion votacion, bool cambio)>();
            
            foreach (var votacion in votaciones)
            {
                // Respetar las pausas manuales - no auto-actualizar votaciones pausadas
                if (votacion.Estado == EstadoVotacion.Pausada)
                    continue;
                    
                var estadoAnterior = votacion.Estado;
                ActualizarEstadoAutomatico(votacion);
                
                if (estadoAnterior != votacion.Estado)
                {
                    votacionesActualizadas.Add((votacion, true));
                }
            }
            
            // Guardar los cambios en la base de datos
            foreach (var (votacion, cambio) in votacionesActualizadas.Where(x => x.cambio))
            {
                await _repo.ActualizarAsync(votacion.Id.ToString(), votacion);
            }
        }

        private Votacion CreateEntityFromDto(CrearVotacionDto dto)
        {
            if (dto.FechaInicio >= dto.FechaFin)
            {
                throw new ArgumentException("La fecha de inicio debe ser menor a la fecha de fin.");
            }

            VotacionFactory factory = dto.Tipo.ToUpper() switch
            {
                "ESTANDAR" => new VotacionEstandarFactory(),
                "MULTICRITERIO" => new VotacionMulticriterioFactory(),
                "MULTICRITERIO_PUBLICO" => new VotacionMulticriterioPublicoFactory(),
                _ => throw new ArgumentException($"Tipo de votación no válido: {dto.Tipo}")
            };

            if (!Guid.TryParse(dto.EventoId, out var eventoGuid))
            {
                throw new ArgumentException("El ID del evento no es válido o no se ha proporcionado.");
            }

            var votacion = factory.Crear(
                dto.Nombre,
                dto.FechaInicio,
                dto.FechaFin,
                dto.LimiteProy,
                dto.Comentarios,
                dto.ComentariosObligatorios,
                eventoGuid,
                dto.EsAnonima
            );

            // Inicializar pausada si la fecha de inicio es en el futuro
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
