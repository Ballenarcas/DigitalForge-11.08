using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IComentarioService
    {
        Task AgregarComentarioAsync(string proyectoId, string texto, Guid? autorId = null);
        Task<List<ComentarioDto>> ObtenerComentariosAsync(string proyectoId, Guid usuarioId, string? votacionId = null);
        Task<List<ComentarioDto>> ObtenerComentariosParaResumenAsync(string proyectoId);
    }
}
