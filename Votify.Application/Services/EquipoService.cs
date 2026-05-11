using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class EquipoService
    {
        private readonly IEquipoRepository _equipoRepository;
        private readonly IParticipanteRepository _participanteRepository;
        private readonly IParticipanteEventoRepository _participanteEventoRepository;

        public EquipoService(IEquipoRepository equipoRepository, IParticipanteRepository participanteRepository, IParticipanteEventoRepository participanteEventoRepository)
        {
            _equipoRepository = equipoRepository;
            _participanteRepository = participanteRepository;
            _participanteEventoRepository = participanteEventoRepository;
        }

        public async Task<Equipo> CrearEquipoAsync(string nombre)
        {
            var equipo = new Equipo(nombre);
            await _equipoRepository.GuardarAsync(equipo);
            return equipo;
        }

        public async Task AsignarParticipanteAEquipoAsync(Guid participanteId, Guid equipoId, Guid eventoId)
        {
            var participante = await _participanteRepository.ObtenerPorIdAsync(participanteId);
            if (participante == null)
            {
                throw new ArgumentException("El participante no existe.");
            }

            var equipo = await _equipoRepository.ObtenerPorIdAsync(equipoId);
            if (equipo == null)
            {
                throw new ArgumentException("El equipo no existe.");
            }

            var rol = await _participanteEventoRepository.ObtenerRolAsync(eventoId, participanteId);
            if (rol != null && (rol.Equals("ORGANIZADOR", StringComparison.OrdinalIgnoreCase) || rol.Equals("Organizador", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Los organizadores de un evento no pueden estar en un equipo para ese mismo evento.");
            }

            participante.EquipoId = equipoId;
            await _participanteRepository.ActualizarAsync(participante);
        }

        public async Task<IEnumerable<Equipo>> ObtenerTodosLosEquiposAsync()
        {
            return await _equipoRepository.ObtenerTodosAsync();
        }

        public async Task<Equipo?> ObtenerEquipoDeParticipanteAsync(Guid participanteId)
        {
            var participante = await _participanteRepository.ObtenerPorIdAsync(participanteId);
            if (participante?.EquipoId == null) return null;

            return await _equipoRepository.ObtenerPorIdAsync(participante.EquipoId.Value);
        }
    }
}
