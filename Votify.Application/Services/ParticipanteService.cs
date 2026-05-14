using System;
using System.Linq;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class ParticipanteService : IParticipanteService
    {
        private readonly IParticipanteRepository _participanteRepository;

        public ParticipanteService(IParticipanteRepository participanteRepository)
        {
            _participanteRepository = participanteRepository;
        }

        public async Task<List<ParticipanteDto>> ObtenerTodosAsync()
        {
            var participantes = await _participanteRepository.ObtenerTodosAsync();
            return participantes.Select(p => new ParticipanteDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Email = p.Email,
                EquipoId = p.EquipoId
            }).ToList();
        }

        public async Task<ParticipanteDto?> ObtenerPorIdAsync(Guid id)
        {
            var p = await _participanteRepository.ObtenerPorIdAsync(id);
            if (p == null) return null;
            return new ParticipanteDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Email = p.Email,
                EquipoId = p.EquipoId
            };
        }
    }
}
