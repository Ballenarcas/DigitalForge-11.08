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

            return entidades.Select(e => new EventoDto
            {
                Id          = e.Id.ToString(),
                Nombre      = e.Nombre,
                Descripcion = e.Descripcion,
                FechaInicio = e.FechaInicio,
                FechaFin    = e.FechaFin,
                ImagenUrl   = e.ImagenUrl
            }).ToList();
        }

        public async Task<List<EventoDto>> ObtenerMisEventosAsync(string participanteId)
        {
            if (!Guid.TryParse(participanteId, out var pId))
            {
                return new List<EventoDto>();
            }

            var entidades = await _repo.ObtenerPorParticipanteAsync(pId);

            return entidades.Select(e => new EventoDto
            {
                Id          = e.Id.ToString(),
                Nombre      = e.Nombre,
                Descripcion = e.Descripcion,
                FechaInicio = e.FechaInicio,
                FechaFin    = e.FechaFin,
                ImagenUrl   = e.ImagenUrl
            }).ToList();
        }

        public async Task<EventoDto?> ObtenerPorIdAsync(string id)
        {
            var e = await _repo.ObtenerPorIdAsync(id);
            if (e is null) return null;

            return new EventoDto
            {
                Id          = e.Id.ToString(),
                Nombre      = e.Nombre,
                Descripcion = e.Descripcion,
                FechaInicio = e.FechaInicio,
                FechaFin    = e.FechaFin,
                ImagenUrl   = e.ImagenUrl
            };
        }

        public async Task<EventoDto> CrearAsync(EventoDto dto, string creadorId)
        {
            var e = new Votify.Domain.Entities.Evento(
                dto.Nombre,
                dto.Descripcion,
                dto.FechaInicio,
                dto.FechaFin,
                dto.ImagenUrl
            );

            await _repo.GuardarAsync(e);

            if (Guid.TryParse(creadorId, out var participanteId))
            {
                var pe = new ParticipanteEvento(participanteId, e.Id, "ADMINISTRADOR");
                await _participanteEventoRepo.GuardarAsync(pe);
            }

            dto.Id = e.Id.ToString();
            return dto;
        }
    }
}
