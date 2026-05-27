using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IEventoService
    {
        Task<List<EventoDto>> ObtenerTodosAsync();
        Task<List<EventoDto>> ObtenerMisEventosAsync(string participanteId);
        Task<EventoDto?> ObtenerPorIdAsync(string id);
        Task<EventoDto> CrearAsync(EventoDto dto, string creadorId);
        Task<EventoDto> ActualizarAsync(EventoDto dto, string solicitanteId);
        Task RegistrarParticipanteAsync(string eventoId, string participanteId);
        Task<string?> ObtenerRolEnEventoAsync(string eventoId, string participanteId);
        Task<Dictionary<string, string>> ObtenerMisRolesAsync(string participanteId);
        Task<List<ParticipanteRolDto>> ObtenerParticipantesPorEventoAsync(string eventoId, string solicitanteId, string? search = null);
        Task<RoleStatisticsDto> ObtenerEstadisticasRolesAsync(string eventoId, string solicitanteId);
        Task CambiarRolParticipanteAsync(string eventoId, string participanteId, string solicitanteId, string rol);
        Task EliminarParticipacionAsync(string eventoId, string participanteId, string solicitanteId);
        Task EliminarEventoAsync(string eventoId, string solicitanteId);
    }
}
