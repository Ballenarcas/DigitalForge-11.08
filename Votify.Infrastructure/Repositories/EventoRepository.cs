using Microsoft.EntityFrameworkCore;
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
            var entities = await _db.Eventos.AsNoTracking().ToListAsync();
            return entities.Select(MapToDomain).ToList();
        }

        public async Task<List<Evento>> ObtenerPorParticipanteAsync(Guid participanteId)
        {
            var eventoIds = await _db.ParticipantesEventos
                .Where(pe => pe.ParticipanteId == participanteId)
                .Select(pe => pe.EventoId)
                .ToListAsync();

            var entities = await _db.Eventos
                .AsNoTracking()
                .Where(e => eventoIds.Contains(e.Id))
                .ToListAsync();

            return entities.Select(MapToDomain).ToList();
        }

        public async Task<Evento?> ObtenerPorIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return null;
            var entity = await _db.Eventos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == guid);
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

            await _db.ParticipantesEventos.Where(pe => pe.EventoId == guid).ExecuteDeleteAsync();

            var votaciones = await _db.Votaciones.Where(v => v.EventoId == guid).ToListAsync();
            foreach (var v in votaciones)
            {
                var votacionId = v.Id;

                var proyectoIds = await _db.Proyectos.Where(p => p.VotacionId == votacionId).Select(p => p.Id).ToListAsync();
                if (proyectoIds.Any())
                {
                    await _db.ValoracionesCriterio.Where(vc => proyectoIds.Contains(vc.ProyectoId)).ExecuteDeleteAsync();
                    foreach (var pId in proyectoIds)
                    {
                        await _db.Comentarios.Where(c => c.Proyecto_Id == pId).ExecuteDeleteAsync();
                    }
                }

                await _db.ManualVotosAsignaciones.Where(a => a.VotacionId == votacionId).ExecuteDeleteAsync();
                await _db.Votos.Where(vt => vt.VotacionId == votacionId).ExecuteDeleteAsync();

                var criterioIds = await _db.Criterios.Where(c => c.VotacionId == votacionId).Select(c => c.Id).ToListAsync();
                if (criterioIds.Any())
                    await _db.ValoracionesCriterio.Where(vc => criterioIds.Contains(vc.CriterioId)).ExecuteDeleteAsync();
                await _db.Criterios.Where(c => c.VotacionId == votacionId).ExecuteDeleteAsync();

                await _db.Proyectos.Where(p => p.VotacionId == votacionId).ExecuteDeleteAsync();
            }
            await _db.Votaciones.Where(v => v.EventoId == guid).ExecuteDeleteAsync();
            await _db.Notificaciones.Where(n => n.RecursoId == id).ExecuteDeleteAsync();

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
            new Evento(
                entity.Nombre,
                entity.Descripcion,
                entity.FechaInicio,
                entity.FechaFin,
                entity.ImagenUrl,
                entity.Id);
    }
}
