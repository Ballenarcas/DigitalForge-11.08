using Microsoft.EntityFrameworkCore;
using Votify.Domain.Builders;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;

namespace Votify.Infrastructure.Repositories
{
    public class EventoRepository : IEventoRepository
    {
        private readonly VotifyDbContext _db;

        public EventoRepository(VotifyDbContext db)
        {
            _db = db;
        }

        public async Task<List<Evento>> ObtenerTodosAsync()
        {
            var entities = await _db.Eventos.ToListAsync();
            return entities.Select(MapToDomain).ToList();
        }

        public async Task<List<Evento>> ObtenerPorParticipanteAsync(Guid participanteId)
        {
            var eventoIds = await _db.ParticipantesEventos
                .Where(pe => pe.ParticipanteId == participanteId)
                .Select(pe => pe.EventoId)
                .ToListAsync();

            var entities = await _db.Eventos
                .Where(e => eventoIds.Contains(e.Id))
                .ToListAsync();

            return entities.Select(MapToDomain).ToList();
        }

        public async Task<Evento?> ObtenerPorIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return null;
            var entity = await _db.Eventos.FindAsync(guid);
            return entity == null ? null : MapToDomain(entity);
        }

        public async Task GuardarAsync(Evento evento)
        {
            var entity = new EventoEntity
            {
                Id          = evento.Id,
                Nombre      = evento.Nombre,
                Descripcion = evento.Descripcion,
                FechaInicio = evento.FechaInicio.ToUniversalTime(),
                FechaFin    = evento.FechaFin.ToUniversalTime(),
                ImagenUrl   = evento.ImagenUrl
            };

            await _db.Eventos.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> EliminarAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return false;
            var entity = await _db.Eventos.FindAsync(guid);
            if (entity is null) return false;

            _db.Eventos.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task ActualizarAsync(Evento evento)
        {
            if (!Guid.TryParse(evento.Id.ToString(), out var guid)) return;
            var entity = await _db.Eventos.FindAsync(guid);
            if (entity is null) return;

            entity.Nombre = evento.Nombre;
            entity.Descripcion = evento.Descripcion;
            entity.FechaInicio = evento.FechaInicio.ToUniversalTime();
            entity.FechaFin = evento.FechaFin.ToUniversalTime();
            entity.ImagenUrl = evento.ImagenUrl;

            await _db.SaveChangesAsync();
        }

        public async Task<bool> ActualizarEventoAsync(Guid eventoId, string nombre, string descripcion, DateTime fechaInicio, DateTime fechaFin, string? imagenUrl)
        {
            var entity = await _db.Eventos.FindAsync(eventoId);
            if (entity is null) return false;

            entity.Nombre = nombre;
            entity.Descripcion = descripcion;
            entity.FechaInicio = fechaInicio.ToUniversalTime();
            entity.FechaFin = fechaFin.ToUniversalTime();
            entity.ImagenUrl = imagenUrl;

            await _db.SaveChangesAsync();
            return true;
        }

        private static Evento MapToDomain(EventoEntity entity) =>
            new EventoBuilder()
                .ConNombre(entity.Nombre)
                .ConDescripcion(entity.Descripcion)
                .ConFechas(entity.FechaInicio, entity.FechaFin)
                .ConImagen(entity.ImagenUrl)
                .ConId(entity.Id)
                .Build();
    }
}
