using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IEventoService
    {
        Task<List<EventoDto>> ObtenerTodosAsync();
        Task<List<EventoDto>> ObtenerMisEventosAsync(string participanteId);
        Task<EventoDto?> ObtenerPorIdAsync(string id);
        Task<EventoDto> CrearAsync(EventoDto dto, string creadorId);
    }
}
