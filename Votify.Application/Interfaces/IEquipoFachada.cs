using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IEquipoFachada
    {
        Task<EquipoDto> CrearEquipoAsync(string nombre);
        Task AsignarParticipanteAsync(Guid solicitanteId, Guid participanteId, Guid equipoId, Guid eventoId);
        Task<List<EquipoDto>> ObtenerTodosAsync();
        Task<EquipoDto?> ObtenerEquipoDeParticipanteAsync(Guid participanteId);
    }
}
