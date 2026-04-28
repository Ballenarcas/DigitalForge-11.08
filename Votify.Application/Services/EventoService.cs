using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _repo;

        public EventoService(IEventoRepository repo)
        {
            _repo = repo;
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

        public async Task<EventoDto> CrearAsync(EventoDto dto)
        {
            var e = new Votify.Domain.Entities.Evento(
                dto.Nombre,
                dto.Descripcion,
                dto.FechaInicio,
                dto.FechaFin,
                dto.ImagenUrl
            );

            await _repo.GuardarAsync(e);

            dto.Id = e.Id.ToString();
            return dto;
        }
    }
}
