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
                FechaFin    = e.FechaFin
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
                FechaFin    = e.FechaFin
            };
        }
    }
}
