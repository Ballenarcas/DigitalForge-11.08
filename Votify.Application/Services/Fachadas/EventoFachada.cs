using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.Application.Services.Fachadas
{
    public class EventoFachada : IEventoFachada
    {
        private readonly IEventoService _eventoService;

        public EventoFachada(IEventoService eventoService)
        {
            _eventoService = eventoService;
        }

        public Task<EventoDto> CrearEventoAsync(EventoDto dto, string creadorId)
            => _eventoService.CrearAsync(dto, creadorId);

        public Task<EventoDto> ActualizarEventoAsync(EventoDto dto, string solicitanteId)
            => _eventoService.ActualizarAsync(dto, solicitanteId);

        public Task<EventoDto?> ObtenerEventoAsync(string id)
            => _eventoService.ObtenerPorIdAsync(id);

        public Task<List<EventoDto>> ObtenerTodosAsync()
            => _eventoService.ObtenerTodosAsync();

        public Task<List<EventoDto>> ObtenerMisEventosAsync(string participanteId)
            => _eventoService.ObtenerMisEventosAsync(participanteId);

        public Task ParticiparEnEventoAsync(string eventoId, string participanteId)
            => _eventoService.RegistrarParticipanteAsync(eventoId, participanteId);

        public Task<string?> ObtenerRolAsync(string eventoId, string participanteId)
            => _eventoService.ObtenerRolEnEventoAsync(eventoId, participanteId);

        public Task<List<ParticipanteRolDto>> ObtenerParticipantesAsync(string eventoId, string solicitanteId, string? search = null)
            => _eventoService.ObtenerParticipantesPorEventoAsync(eventoId, solicitanteId, search);

        public Task<RoleStatisticsDto> ObtenerEstadisticasRolesAsync(string eventoId, string solicitanteId)
            => _eventoService.ObtenerEstadisticasRolesAsync(eventoId, solicitanteId);

        public Task CambiarRolAsync(string eventoId, string participanteId, string solicitanteId, string rol)
            => _eventoService.CambiarRolParticipanteAsync(eventoId, participanteId, solicitanteId, rol);

        public Task EliminarParticipacionAsync(string eventoId, string participanteId, string solicitanteId)
            => _eventoService.EliminarParticipacionAsync(eventoId, participanteId, solicitanteId);

        public Task EliminarEventoAsync(string eventoId, string solicitanteId)
            => _eventoService.EliminarEventoAsync(eventoId, solicitanteId);
    }
}
