using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.Application.Services.Fachadas
{
    public class VotoFachada : IVotoFachada
    {
        private readonly IVotoService _votoService;

        public VotoFachada(IVotoService votoService)
        {
            _votoService = votoService;
        }

        public Task VotarAsync(VotarDto dto)
            => _votoService.VotarAsync(dto);

        public Task VotarMulticriterioAsync(VotoMulticriterioDto dto)
            => _votoService.VotarMulticriterioAsync(dto);

        public Task<bool> PuedeVotarAsync(string votacionId, string votanteId)
            => _votoService.PuedeVotarAsync(votacionId, votanteId);

        public Task<bool> HaVotadoMulticriterioAsync(string proyectoId, string votanteId)
            => _votoService.HaVotadoMulticriterioAsync(proyectoId, votanteId);
    }
}
