using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IEventoService
    {
        Task<List<EventoDto>> ObtenerTodosAsync();
        Task<EventoDto?> ObtenerPorIdAsync(string id);
    }
}
