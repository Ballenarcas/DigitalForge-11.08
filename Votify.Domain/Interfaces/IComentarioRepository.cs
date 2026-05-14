using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IComentarioRepository
    {
        Task GuardarAsync(string proyectoId, string texto, Guid? autorId = null);
        Task GuardarAnonimoAsync(string proyectoId, string texto);
        Task<List<Comentario>> ObtenerAsync(string proyectoId);
        Task<bool> HaComentadoProyectoAsync(string proyectoId, Guid autorId);
    }
}
