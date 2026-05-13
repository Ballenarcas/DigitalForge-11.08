using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IVotoFachada
    {
        Task VotarAsync(VotarDto dto);
        Task VotarMulticriterioAsync(VotoMulticriterioDto dto);
        Task<bool> PuedeVotarAsync(string votacionId, string votanteId);
        Task<bool> HaVotadoMulticriterioAsync(string proyectoId, string votanteId);
    }
}
