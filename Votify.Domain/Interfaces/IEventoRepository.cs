using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IEventoRepository
    {
        Task<List<Evento>> ObtenerTodosAsync();
        Task<List<Evento>> ObtenerPorParticipanteAsync(Guid participanteId);
        Task<Evento?> ObtenerPorIdAsync(string id);
        Task GuardarAsync(Evento evento);
        Task<bool> EliminarAsync(string id);
        Task ActualizarAsync(Evento evento);
        Task<bool> ActualizarEventoAsync(Guid eventoId, string nombre, string descripcion, DateTime fechaInicio, DateTime fechaFin, string? imagenUrl);
    }
}
