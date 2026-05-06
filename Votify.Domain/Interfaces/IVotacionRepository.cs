using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IVotacionRepository
    {
        Task GuardarAsync(Votacion votacion);
        Task<Votacion?> ObtenerAsync(string id);
        Task<List<Votacion>> ObtenerTodasAsync();
        Task<List<Votacion>> ObtenerPorEventoAsync(Guid eventoId);
        Task<bool> ActualizarAsync(string id, Votacion votacion);
        Task ActualizarEstadoAsync(string id, EstadoVotacion estado);
        Task<bool> EliminarAsync(string id);
        Task<string?> ObtenerEventoIdAsync(string votacionId);
    }   
}
