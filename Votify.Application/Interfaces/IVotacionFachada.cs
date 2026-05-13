using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IVotacionFachada
    {
        Task CrearVotacionAsync(CrearVotacionDto dto);
        Task<CrearVotacionResponse?> ObtenerVotacionAsync(string id);
        Task<List<CrearVotacionResponse>> ObtenerVotacionesAsync();
        Task<List<CrearVotacionResponse>> ObtenerVotacionesPorEventoAsync(string eventoId);
        Task ActualizarVotacionAsync(string id, CrearVotacionDto dto);
        Task EliminarVotacionAsync(string id);
        Task PausarVotacionAsync(string id);
        Task DetenerVotacionAsync(string id);
        Task AbrirVotacionAsync(string id);
        Task<List<ResultadoProyectoDto>> ObtenerResultadosAsync(string votacionId);
        Task<List<ResultadoMulticriterioDto>> ObtenerResultadosMulticriterioAsync(string votacionId);
        Task<bool> PuedeVotarAsync(string votacionId, string votanteId);
        Task<bool> HaVotadoMulticriterioAsync(string proyectoId, string votanteId);
    }
}
