using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.Application.Services.Fachadas
{
    public class EquipoFachada : IEquipoFachada
    {
        private readonly IEquipoService _equipoService;

        public EquipoFachada(IEquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        public Task<EquipoDto> CrearEquipoAsync(string nombre)
            => _equipoService.CrearEquipoAsync(nombre);

        public Task AsignarParticipanteAsync(Guid solicitanteId, Guid participanteId, Guid equipoId, Guid eventoId)
            => _equipoService.AsignarParticipanteAEquipoAsync(solicitanteId, participanteId, equipoId, eventoId);

        public Task<List<EquipoDto>> ObtenerTodosAsync()
            => _equipoService.ObtenerTodosLosEquiposAsync();

        public Task<EquipoDto?> ObtenerEquipoDeParticipanteAsync(Guid participanteId)
            => _equipoService.ObtenerEquipoDeParticipanteAsync(participanteId);
    }
}
