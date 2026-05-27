using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IEventoFachada
    {
        Task<EventoDto> CrearEventoAsync(EventoDto dto, string creadorId);
        Task<EventoDto> ActualizarEventoAsync(EventoDto dto, string solicitanteId);
        Task<EventoDto?> ObtenerEventoAsync(string id);
        Task<List<EventoDto>> ObtenerTodosAsync();
        Task<List<EventoDto>> ObtenerMisEventosAsync(string participanteId);
        Task ParticiparEnEventoAsync(string eventoId, string participanteId);
        Task<string?> ObtenerRolAsync(string eventoId, string participanteId);
        Task<Dictionary<string, string>> ObtenerMisRolesAsync(string participanteId);
        Task<List<ParticipanteRolDto>> ObtenerParticipantesAsync(string eventoId, string solicitanteId, string? search = null);
        Task<RoleStatisticsDto> ObtenerEstadisticasRolesAsync(string eventoId, string solicitanteId);
        Task CambiarRolAsync(string eventoId, string participanteId, string solicitanteId, string rol);
        Task EliminarParticipacionAsync(string eventoId, string participanteId, string solicitanteId);
        Task EliminarEventoAsync(string eventoId, string solicitanteId);
    }
}
