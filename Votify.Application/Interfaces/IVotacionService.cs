using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IVotacionService
    {
        Task CrearVotacionAsync(CrearVotacionDto dto);
        Task<List<CrearVotacionResponse>> ObtenerTodasAsync();
        Task<List<CrearVotacionResponse>> ObtenerPorEventoAsync(string eventoId);
        Task<CrearVotacionResponse?> ObtenerPorIdAsync(string id);
        Task ActualizarVotacionAsync(string id, CrearVotacionDto dto);
        Task EliminarVotacionAsync(string id);
        Task<List<ResultadoProyectoDto>> ObtenerResultadosAsync(string votacionId);
        Task<List<ResultadoMulticriterioDto>> ObtenerResultadosMulticriterioAsync(string votacionId);
        Task PausarVotacionAsync(string id);
        Task DetenerVotacionAsync(string id);
        Task AbrirVotacionAsync(string id);
    }
}
