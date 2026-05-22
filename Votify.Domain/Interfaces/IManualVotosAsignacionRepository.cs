using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IManualVotosAsignacionRepository
    {
        Task GuardarAsync(ManualVotosAsignacion asignacion);
        Task<List<ManualVotosAsignacion>> ObtenerPorVotacionAsync(Guid votacionId);
        Task<ManualVotosAsignacion?> ObtenerPorIdAsync(Guid id);
        Task EliminarAsync(Guid votacionId, Guid proyectoId);
    }
}
