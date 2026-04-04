using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IVotacionService
    {
        Task CrearVotacionAsync(CrearVotacionDto dto);
        Task<List<CrearVotacionResponse>> ObtenerTodasAsync();
        Task<CrearVotacionResponse?> ObtenerPorIdAsync(string id);
        Task ActualizarVotacionAsync(string id, CrearVotacionDto dto);
        Task EliminarVotacionAsync(string id);
        Task<List<ResultadoProyectoDto>> ObtenerResultadosAsync(string votacionId);
    }
}
