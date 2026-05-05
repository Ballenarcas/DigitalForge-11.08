using System;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _repo;
        private readonly IParticipanteEventoRepository _participanteEventoRepo;

        public EventoService(IEventoRepository repo, IParticipanteEventoRepository participanteEventoRepo)
        {
            _repo = repo;
            _participanteEventoRepo = participanteEventoRepo;
        }

        public async Task<List<EventoDto>> ObtenerTodosAsync()
        {
            var entidades = await _repo.ObtenerTodosAsync();

            return entidades.Select(MapToEventoDTO).ToList();
        }

        public async Task<List<EventoDto>> ObtenerMisEventosAsync(string participanteId)
        {
            if (!Guid.TryParse(participanteId, out var pId))
            {
                return new List<EventoDto>();
            }

            var entidades = await _repo.ObtenerPorParticipanteAsync(pId);

            return entidades.Select(MapToEventoDTO).ToList();
        }

        public async Task<EventoDto?> ObtenerPorIdAsync(string id)
        {
            var e = await _repo.ObtenerPorIdAsync(id);
            if (e is null) return null;

            return MapToEventoDTO(e);
        }

        public async Task<EventoDto> CrearAsync(EventoDto dto, string creadorId)
        {
            var e = new Evento(
                dto.Nombre,
                dto.Descripcion,
                dto.FechaInicio,
                dto.FechaFin,
                dto.ImagenUrl
            );

            await _repo.GuardarAsync(e);

            if (Guid.TryParse(creadorId, out var participanteId))
            {
                var pe = new ParticipanteEvento(participanteId, e.Id, "ORGANIZADOR");
                await _participanteEventoRepo.GuardarAsync(pe);
            }

            dto.Id = e.Id.ToString();
            return dto;
        }

        public async Task RegistrarParticipanteAsync(string eventoId, string participanteId)
        {
            if (Guid.TryParse(eventoId, out var eId) && Guid.TryParse(participanteId, out var pId))
            {
                // Verificar si ya está participando
                var misEventos = await _repo.ObtenerPorParticipanteAsync(pId);
                if (!misEventos.Any(x => x.Id == eId))
                {
                    var pe = new ParticipanteEvento(pId, eId, "VOTANTE");
                    await _participanteEventoRepo.GuardarAsync(pe);
                }
            }
        }

        public async Task<string?> ObtenerRolEnEventoAsync(string eventoId, string participanteId)
        {
            if (Guid.TryParse(eventoId, out var eId) && Guid.TryParse(participanteId, out var pId))
            {
                return await _participanteEventoRepo.ObtenerRolAsync(eId, pId);
            }
            return null;
        }

        private static EventoDto MapToEventoDTO(Evento e) => new EventoDto
        {
            Id          = e.Id.ToString(),
            Nombre      = e.Nombre,
            Descripcion = e.Descripcion,
            FechaInicio = e.FechaInicio,
            FechaFin    = e.FechaFin,
            ImagenUrl   = e.ImagenUrl
        };
    }
}
