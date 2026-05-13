using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.Application.Services.Fachadas
{
    public class VotacionFachada : IVotacionFachada
    {
        private readonly IVotacionService _votacionService;
        private readonly IVotoService _votoService;

        public VotacionFachada(IVotacionService votacionService, IVotoService votoService)
        {
            _votacionService = votacionService;
            _votoService = votoService;
        }

        public Task CrearVotacionAsync(CrearVotacionDto dto)
            => _votacionService.CrearVotacionAsync(dto);

        public Task<CrearVotacionResponse?> ObtenerVotacionAsync(string id)
            => _votacionService.ObtenerPorIdAsync(id);

        public Task<List<CrearVotacionResponse>> ObtenerVotacionesAsync()
            => _votacionService.ObtenerTodasAsync();

        public Task<List<CrearVotacionResponse>> ObtenerVotacionesPorEventoAsync(string eventoId)
            => _votacionService.ObtenerPorEventoAsync(eventoId);

        public Task ActualizarVotacionAsync(string id, CrearVotacionDto dto)
            => _votacionService.ActualizarVotacionAsync(id, dto);

        public Task EliminarVotacionAsync(string id)
            => _votacionService.EliminarVotacionAsync(id);

        public Task PausarVotacionAsync(string id)
            => _votacionService.PausarVotacionAsync(id);

        public Task DetenerVotacionAsync(string id)
            => _votacionService.DetenerVotacionAsync(id);

        public Task AbrirVotacionAsync(string id)
            => _votacionService.AbrirVotacionAsync(id);

        public Task<List<ResultadoProyectoDto>> ObtenerResultadosAsync(string votacionId)
            => _votacionService.ObtenerResultadosAsync(votacionId);

        public Task<List<ResultadoMulticriterioDto>> ObtenerResultadosMulticriterioAsync(string votacionId)
            => _votacionService.ObtenerResultadosMulticriterioAsync(votacionId);

        public Task<bool> PuedeVotarAsync(string votacionId, string votanteId)
            => _votoService.PuedeVotarAsync(votacionId, votanteId);

        public Task<bool> HaVotadoMulticriterioAsync(string proyectoId, string votanteId)
            => _votoService.HaVotadoMulticriterioAsync(proyectoId, votanteId);
    }
}
