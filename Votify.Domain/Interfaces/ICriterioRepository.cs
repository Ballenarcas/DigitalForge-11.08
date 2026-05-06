using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface ICriterioRepository
    {
        Task<List<Criterio>> ObtenerPorVotacionAsync(string votacionId);
        Task ReemplazarPorVotacionAsync(string votacionId, List<Criterio> criterios);
        Task EliminarPorVotacionAsync(string votacionId);
    }
}
