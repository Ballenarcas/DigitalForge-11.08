using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IEventoRepository
    {
        Task<List<Evento>> ObtenerTodosAsync();
        Task<Evento?> ObtenerPorIdAsync(string id);
        Task GuardarAsync(Evento evento);
        Task<bool> EliminarAsync(string id);
    }
}
