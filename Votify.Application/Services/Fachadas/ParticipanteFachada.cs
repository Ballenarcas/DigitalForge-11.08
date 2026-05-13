using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.Application.Services.Fachadas
{
    public class ParticipanteFachada : IParticipanteFachada
    {
        private readonly IParticipanteService _participanteService;

        public ParticipanteFachada(IParticipanteService participanteService)
        {
            _participanteService = participanteService;
        }

        public Task<List<ParticipanteDto>> ObtenerTodosAsync()
            => _participanteService.ObtenerTodosAsync();

        public Task<ParticipanteDto?> ObtenerPorIdAsync(Guid id)
            => _participanteService.ObtenerPorIdAsync(id);
    }
}
