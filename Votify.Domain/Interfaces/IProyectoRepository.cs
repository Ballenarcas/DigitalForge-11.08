using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IProyectoRepository
    {
        Task GuardarAsync(Proyecto proyecto);
        Task<List<Proyecto>> ObtenerTodasAsync();
        Task<Proyecto?> ObtenerAsync(string proyectoId);
        Task<List<Proyecto>> ObtenerPorVotacionAsync(string votacionId);
    }
}