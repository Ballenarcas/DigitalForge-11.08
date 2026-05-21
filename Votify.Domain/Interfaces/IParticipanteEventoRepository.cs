using System.Threading.Tasks;
using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IParticipanteEventoRepository
    {
        Task GuardarAsync(ParticipanteEvento participanteEvento);
        Task<string?> ObtenerRolAsync(Guid eventoId, Guid participanteId);
        Task<List<ParticipanteEventoDetalle>> ObtenerParticipantesPorEventoAsync(Guid eventoId, string? search = null);
        Task<RoleStatistics> ContarRolesPorEventoAsync(Guid eventoId);
        Task<int> ContarParticipantesConRolAsync(Guid eventoId);
        Task<bool> ActualizarRolAsync(Guid eventoId, Guid participanteId, string rol);
        Task<bool> EliminarAsync(Guid eventoId, Guid participanteId);
        Task<List<Guid>> ObtenerOrganizadoresIdsAsync(Guid eventoId);
    }
}
