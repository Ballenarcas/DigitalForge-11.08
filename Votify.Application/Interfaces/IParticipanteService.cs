using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IParticipanteService
    {
        Task<List<ParticipanteDto>> ObtenerTodosAsync();
        Task<ParticipanteDto?> ObtenerPorIdAsync(Guid id);
    }
}
