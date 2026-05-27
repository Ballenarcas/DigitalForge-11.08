using Votify.Domain.Entities;
using System.Collections.Generic;

namespace Votify.Domain.Interfaces
{
    public interface ICriterioRepository
    {
        Task<List<Criterio>> ObtenerPorVotacionAsync(string votacionId);
        Task<Dictionary<string, List<Criterio>>> ObtenerPorVotacionesAsync(IEnumerable<string> votacionIds);
        Task ReemplazarPorVotacionAsync(string votacionId, List<Criterio> criterios);
        Task EliminarPorVotacionAsync(string votacionId);
    }
}
