using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IParticipanteFachada
    {
        Task<List<ParticipanteDto>> ObtenerTodosAsync();
    }
}
