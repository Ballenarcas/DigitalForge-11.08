using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IProyectoFachada
    {
        Task<string> CrearProyectoAsync(ProyectoDto dto);
        Task<ProyectoDto?> ObtenerProyectoAsync(string id);
        Task<List<ProyectoDto>> ObtenerProyectosAsync();
        Task<List<ProyectoDto>> ObtenerProyectosPorVotacionAsync(string votacionId);
        Task AgregarComentarioAsync(string proyectoId, string texto, Guid? autorId = null);
        Task<List<ComentarioDto>> ObtenerComentariosAsync(string proyectoId, Guid usuarioId, string? votacionId = null);
    }
}
