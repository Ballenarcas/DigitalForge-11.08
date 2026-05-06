using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IVotoRepository
    {
        Task GuardarAsync(Voto voto);
        Task GuardarAsync(Voto voto, int? puntuacion);
        Task<List<Voto>> ObtenerPorProyectoAsync(string proyectoId);
        Task<int> ContarVotosPorUsuarioYVotacionAsync(string votacionId, string votanteId);
        Task<bool> HaVotadoPorProyectoAsync(string votacionId, string proyectoId, string votanteId);
        Task<List<(string ProyectoId, int Votos)>> ObtenerVotosPorVotacionAsync(string votacionId);
        Task<bool> EliminarPorVotacionAsync(string votacionId);
    }
}
