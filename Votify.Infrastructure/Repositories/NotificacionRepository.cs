using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;

namespace Votify.Infrastructure.Repositories
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly VotifyDbContext _context;

        public NotificacionRepository(VotifyDbContext context)
        {
            _context = context;
        }

        public async Task GuardarAsync(Notificacion notificacion)
        {
            var entity = new NotificacionEntity
            {
                Id = notificacion.Id,
                UsuarioId = notificacion.UsuarioId,
                Mensaje = notificacion.Mensaje,
                Tipo = notificacion.Tipo,
                RecursoId = notificacion.RecursoId,
                RecursoTipo = notificacion.RecursoTipo,
                Leida = notificacion.Leida,
                CreatedAt = notificacion.CreatedAt
            };

            await _context.Notificaciones.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notificacion>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            var entities = await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return entities.Select(e => new Notificacion(
                e.UsuarioId,
                e.Mensaje,
                e.Tipo,
                e.RecursoId,
                e.RecursoTipo,
                e.Id,
                e.CreatedAt
            ) { Leida = e.Leida }).ToList();
        }

        public async Task<Notificacion?> ObtenerPorIdAsync(Guid id)
        {
            var entity = await _context.Notificaciones.FindAsync(id);
            if (entity == null) return null;

            return new Notificacion(
                entity.UsuarioId,
                entity.Mensaje,
                entity.Tipo,
                entity.RecursoId,
                entity.RecursoTipo,
                entity.Id,
                entity.CreatedAt
            ) { Leida = entity.Leida };
        }

        public async Task MarcarComoLeidaAsync(Guid id)
        {
            var entity = await _context.Notificaciones.FindAsync(id);
            if (entity != null)
            {
                entity.Leida = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> ObtenerNoLeidasCountAsync(Guid usuarioId)
        {
            return await _context.Notificaciones
                .CountAsync(n => n.UsuarioId == usuarioId && !n.Leida);
        }
    }
}