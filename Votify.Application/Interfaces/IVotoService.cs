using System.Threading.Tasks;
using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IVotoService
    {
        Task VotarAsync(VotarDto dto);
        Task VotarMulticriterioAsync(VotoMulticriterioDto dto);
        Task VotarMulticriterioAnonimoAsync(VotoMulticriterioAnonimoDto dto);
        Task<bool> HaVotadoMulticriterioAsync(string proyectoId, string votanteId);
        Task<bool> PuedeVotarAsync(string votacionId, string votanteId);
    }
}
