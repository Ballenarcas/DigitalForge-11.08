using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IVotoRepository
    {
        Task GuardarAsync(Voto voto);
        Task<List<Voto>> ObtenerPorProyectoAsync(string proyectoId);
        Task<int> ContarVotosPorUsuarioYVotacionAsync(string votacionId, string votanteId);
        Task<List<(string ProyectoId, int Votos)>> ObtenerVotosPorVotacionAsync(string votacionId);
    }
}
