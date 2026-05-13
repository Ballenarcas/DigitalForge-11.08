using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class EquipoService : IEquipoService
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

        public async Task<EquipoDto> CrearEquipoAsync(string nombre)
        {
            var equipo = new Equipo(nombre);
            await _equipoRepository.GuardarAsync(equipo);
            return MapToDto(equipo);
        }

        public async Task AsignarParticipanteAEquipoAsync(Guid solicitanteId, Guid participanteId, Guid equipoId, Guid eventoId)
        {
            var equipo = await _equipoRepository.ObtenerPorIdAsync(equipoId);
            if (equipo == null)
            {
                throw new ArgumentException("El equipo no existe.");
            }

            var participante = await _participanteRepository.ObtenerPorIdAsync(participanteId);
            if (participante == null)
            {
                throw new ArgumentException("El participante no existe.");
            }

            if (!await PuedeGestionarEquipoAsync(solicitanteId, equipoId, eventoId))
            {
                throw new UnauthorizedAccessException("No tienes permisos para agregar participantes a este equipo.");
            }

            participante.EquipoId = equipoId;
            await _participanteRepository.ActualizarAsync(participante);

            var rol = await _participanteEventoRepository.ObtenerRolAsync(eventoId, participanteId);

            if (rol == null)
            {
                await _participanteEventoRepository.GuardarAsync(new ParticipanteEvento(participanteId, eventoId, "COMPETIDOR"));
            }
            else if (!string.Equals(rol, "COMPETIDOR", StringComparison.OrdinalIgnoreCase))
            {
                await _participanteEventoRepository.ActualizarRolAsync(eventoId, participanteId, "COMPETIDOR");
            }
        }

        private async Task<bool> PuedeGestionarEquipoAsync(Guid solicitanteId, Guid equipoId, Guid eventoId)
        {
            var rolSolicitante = await _participanteEventoRepository.ObtenerRolAsync(eventoId, solicitanteId);
            if (EsOrganizador(rolSolicitante))
            {
                return true;
            }

            var solicitante = await _participanteRepository.ObtenerPorIdAsync(solicitanteId);
            return solicitante?.EquipoId == equipoId;
        }

        private static bool EsOrganizador(string? rol)
        {
            return string.Equals(rol?.Trim(), "ORGANIZADOR", StringComparison.OrdinalIgnoreCase)
                || string.Equals(rol?.Trim(), "Organizador", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<EquipoDto>> ObtenerTodosLosEquiposAsync()
        {
            var equipos = await _equipoRepository.ObtenerTodosAsync();
            return equipos.Select(MapToDto).ToList();
        }

        public async Task<EquipoDto?> ObtenerEquipoDeParticipanteAsync(Guid participanteId)
        {
            var participante = await _participanteRepository.ObtenerPorIdAsync(participanteId);
            if (participante?.EquipoId == null) return null;

            var equipo = await _equipoRepository.ObtenerPorIdAsync(participante.EquipoId.Value);
            return equipo != null ? MapToDto(equipo) : null;
        }

        private static EquipoDto MapToDto(Equipo equipo) => new EquipoDto
        {
            Id = equipo.Id,
            Nombre = equipo.Nombre,
            CreatedAt = equipo.CreatedAt
        };
    }
}
