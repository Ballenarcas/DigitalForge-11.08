using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IVotacionRepository
    {
        Task GuardarAsync(Votacion votacion);
        Task<Votacion?> ObtenerAsync(string id);
        Task<List<Votacion>> ObtenerTodasAsync();
        Task<bool> ActualizarAsync(string id, Votacion votacion);
        Task<bool> EliminarAsync(string id);
    }   
}
