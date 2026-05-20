using Votify.Application.DTOs;
using Votify.Domain.Entities;
using Votify.Domain.Factory;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services.Estrategia
{
    public class VotacionEstandarStrategy : IVotacionStrategy
    {
        private readonly IVotoRepository _votoRepository;
        private readonly IProyectoRepository _proyectoRepository;
        private readonly IEquipoRepository _equipoRepository;

        public string Tipo => "ESTANDAR";

        public VotacionEstandarStrategy(
            IVotoRepository votoRepository,
            IProyectoRepository proyectoRepository,
            IEquipoRepository equipoRepository)
        {
            _votoRepository = votoRepository;
            _proyectoRepository = proyectoRepository;
            _equipoRepository = equipoRepository;
        }

        public async Task ProcesarVotoAsync(Votacion votacion, VotarDto dto)
        {
            int votosActuales = await _votoRepository.ContarVotosPorUsuarioYVotacionAsync(dto.VotacionId, dto.VotanteId ?? string.Empty);
            if (votosActuales >= votacion.LimiteProy)
            {
                throw new InvalidOperationException($"No puedes votar. Has alcanzado el límite de {votacion.LimiteProy} votos para esta votación.");
            }

            if (!string.IsNullOrEmpty(dto.VotanteId))
            {
                bool haVotado = await _votoRepository.HaVotadoPorProyectoAsync(dto.VotacionId, dto.ProyectoId, dto.VotanteId);
                if (haVotado)
                {
                    throw new InvalidOperationException("Ya has votado por este proyecto en esta votacion.");
                }
            }

            VotoFactory factory = string.IsNullOrEmpty(dto.VotanteId)
                ? new VotoAnonimoFactory()
                : new VotoEstandarFactory();

            var nuevoVoto = factory.Crear(dto.ProyectoId, dto.VotacionId, dto.VotanteId);
            await _votoRepository.GuardarAsync(nuevoVoto);
        }

        public Task ProcesarVotoMulticriterioAsync(Votacion votacion, VotoMulticriterioDto dto)
        {
            throw new NotSupportedException("La votación estándar no admite votos multicriterio.");
        }

        public Task ProcesarVotoMulticriterioAnonimoAsync(Votacion votacion, VotoMulticriterioAnonimoDto dto)
        {
            throw new NotSupportedException("La votación estándar no admite votos multicriterio anónimos.");
        }

        public async Task<bool> HaVotadoAsync(string votacionId, string proyectoId, string votanteId)
        {
            return await _votoRepository.HaVotadoPorProyectoAsync(votacionId, proyectoId, votanteId);
        }

        public async Task<List<ResultadoProyectoDto>> CalcularResultadosAsync(string votacionId)
        {
            var votosPorProyecto = await _votoRepository.ObtenerVotosPorVotacionAsync(votacionId);

            if (votosPorProyecto.Count == 0)
                return new List<ResultadoProyectoDto>();

            var proyectos = await _proyectoRepository.ObtenerPorVotacionAsync(votacionId);
            var proyectoDict = proyectos.ToDictionary(p => p.Id);

            var equipos = await _equipoRepository.ObtenerTodosAsync();
            var equipoDict = equipos.ToDictionary(e => e.Id.ToString(), e => e.Nombre);

            var resultados = votosPorProyecto
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

        public Task<List<ResultadoMulticriterioDto>> CalcularResultadosMulticriterioAsync(string votacionId)
        {
            return Task.FromResult(new List<ResultadoMulticriterioDto>());
        }
    }
}
