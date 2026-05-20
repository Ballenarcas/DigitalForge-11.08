using Votify.Application.DTOs;
using Votify.Domain.Entities;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services.Estrategia
{
    public class VotacionMulticriterioPublicoStrategy : IVotacionStrategy
    {
        private readonly ICriterioRepository _criterioRepository;
        private readonly IValoracionCriterioRepository _valoracionCriterioRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IProyectoRepository _proyectoRepository;
        private readonly IEquipoRepository _equipoRepository;

        public string Tipo => "MULTICRITERIO_PUBLICO";

        public VotacionMulticriterioPublicoStrategy(
            ICriterioRepository criterioRepository,
            IValoracionCriterioRepository valoracionCriterioRepository,
            IComentarioRepository comentarioRepository,
            IProyectoRepository proyectoRepository,
            IEquipoRepository equipoRepository)
        {
            _criterioRepository = criterioRepository;
            _valoracionCriterioRepository = valoracionCriterioRepository;
            _comentarioRepository = comentarioRepository;
            _proyectoRepository = proyectoRepository;
            _equipoRepository = equipoRepository;
        }

        public Task ProcesarVotoAsync(Votacion votacion, VotarDto dto)
        {
            throw new NotSupportedException("La votación multicriterio pública no admite votos estándar.");
        }

        public Task ProcesarVotoMulticriterioAsync(Votacion votacion, VotoMulticriterioDto dto)
        {
            throw new NotSupportedException("La votación multicriterio pública no admite votos multicriterio autenticados directamente. Use el endpoint anónimo.");
        }

        public async Task ProcesarVotoMulticriterioAnonimoAsync(Votacion votacion, VotoMulticriterioAnonimoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Comentario))
            {
                throw new InvalidOperationException("El comentario es obligatorio en una votación multicriterio.");
            }

            var criterios = await _criterioRepository.ObtenerPorVotacionAsync(dto.VotacionId);
            if (!criterios.Any())
            {
                throw new InvalidOperationException("La votación multicriterio no tiene criterios configurados.");
            }

            if (dto.Valoraciones.Count != criterios.Count)
            {
                throw new InvalidOperationException("Debes valorar todos los criterios.");
            }

            var criterioIds = criterios.Select(c => c.Id).ToHashSet();
            var votanteIdAnonimo = Guid.NewGuid().ToString();
            var valoraciones = dto.Valoraciones.Select(v =>
            {
                if (!Guid.TryParse(v.CriterioId, out var criterioId) || !criterioIds.Contains(criterioId))
                {
                    throw new InvalidOperationException("Uno de los criterios no pertenece a esta votación.");
                }

                if (v.Valoracion < 1 || v.Valoracion > 5)
                {
                    throw new InvalidOperationException("Las valoraciones multicriterio deben estar entre 1 y 5.");
                }

                return new ValoracionCriterio
                {
                    CriterioId = criterioId,
                    Valoracion = v.Valoracion
                };
            }).ToList();

            await _valoracionCriterioRepository.GuardarAsync(dto.ProyectoId, votanteIdAnonimo, valoraciones);
            await _comentarioRepository.GuardarAnonimoAsync(dto.ProyectoId, dto.Comentario);
        }

        public async Task<bool> HaVotadoAsync(string votacionId, string proyectoId, string votanteId)
        {
            return await _valoracionCriterioRepository.HaValoradoProyectoAsync(proyectoId, votanteId);
        }

        public async Task<List<ResultadoProyectoDto>> CalcularResultadosAsync(string votacionId)
        {
            var equipos = await _equipoRepository.ObtenerTodosAsync();
            var equipoDict = equipos.ToDictionary(e => e.Id.ToString(), e => e.Nombre);

            var ponderados = await _valoracionCriterioRepository.ObtenerResultadosPonderadosAsync(votacionId);
            var proyectos = await _proyectoRepository.ObtenerPorVotacionAsync(votacionId);
            var proyectoDict = proyectos.ToDictionary(p => p.Id);

            return ponderados.Select((resultado, index) => new ResultadoProyectoDto
            {
                Id = resultado.ProyectoId,
                Nombre = proyectoDict.ContainsKey(resultado.ProyectoId) ? proyectoDict[resultado.ProyectoId].Nombre : "Proyecto desconocido",
                Equipo = proyectoDict.ContainsKey(resultado.ProyectoId) && proyectoDict[resultado.ProyectoId].Equipo_Id != null && equipoDict.ContainsKey(proyectoDict[resultado.ProyectoId].Equipo_Id!)
                    ? equipoDict[proyectoDict[resultado.ProyectoId].Equipo_Id!]
                    : "Sin equipo",
                TotalVotos = resultado.Evaluaciones,
                PuntajeFinal = Math.Round(resultado.Puntaje, 2),
                Evaluaciones = resultado.Evaluaciones,
                Posicion = index + 1
            }).ToList();
        }

        public async Task<List<ResultadoMulticriterioDto>> CalcularResultadosMulticriterioAsync(string votacionId)
        {
            var equipos = await _equipoRepository.ObtenerTodosAsync();
            var equipoDict = equipos.ToDictionary(e => e.Id.ToString(), e => e.Nombre);

            var ponderados = await _valoracionCriterioRepository.ObtenerResultadosPonderadosAsync(votacionId);
            var detalles = await _valoracionCriterioRepository.ObtenerDetallesPorCriterioAsync(votacionId);
            var criterios = await _criterioRepository.ObtenerPorVotacionAsync(votacionId);
            var criterioDict = criterios.ToDictionary(c => c.Id.ToString(), c => c);
            var proyectos = await _proyectoRepository.ObtenerPorVotacionAsync(votacionId);
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
    }
}
