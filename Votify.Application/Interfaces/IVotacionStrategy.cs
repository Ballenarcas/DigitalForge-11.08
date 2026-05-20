using Votify.Application.DTOs;
using Votify.Domain.Entities;

namespace Votify.Application.Interfaces
{
    public interface IVotacionStrategy
    {
        string Tipo { get; }

        Task ProcesarVotoAsync(Votacion votacion, VotarDto dto);
        Task ProcesarVotoMulticriterioAsync(Votacion votacion, VotoMulticriterioDto dto);
        Task ProcesarVotoMulticriterioAnonimoAsync(Votacion votacion, VotoMulticriterioAnonimoDto dto);

        Task<bool> HaVotadoAsync(string votacionId, string proyectoId, string votanteId);

        Task<List<ResultadoProyectoDto>> CalcularResultadosAsync(string votacionId);
        Task<List<ResultadoMulticriterioDto>> CalcularResultadosMulticriterioAsync(string votacionId);
    }
}
