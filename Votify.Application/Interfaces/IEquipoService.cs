using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IEquipoService
    {
        Task<EquipoDto> CrearEquipoAsync(string nombre);
        Task AsignarParticipanteAEquipoAsync(Guid solicitanteId, Guid participanteId, Guid equipoId, Guid eventoId);
        Task<List<EquipoDto>> ObtenerTodosLosEquiposAsync();
        Task<EquipoDto?> ObtenerEquipoDeParticipanteAsync(Guid participanteId);
    }
}
