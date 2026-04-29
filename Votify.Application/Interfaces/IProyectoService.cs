using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IProyectoService
    {
        Task<string> CrearProyectoAsync(ProyectoDto dto);
        Task<ProyectoDto?> ObtenerProyectoAsync(string id);
        Task<List<ProyectoDto>> ObtenerProyectosAsync();
        Task<List<ProyectoDto>> ObtenerProyectosPorVotacionAsync(string votacionId);
    }
}