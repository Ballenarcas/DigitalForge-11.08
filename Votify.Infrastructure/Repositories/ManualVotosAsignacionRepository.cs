using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Votify.Infrastructure.Repositories
{
    public class ManualVotosAsignacionRepository : IManualVotosAsignacionRepository
    {
        private readonly VotifyDbContext _context;

        public ManualVotosAsignacionRepository(VotifyDbContext context)
        {
            _context = context;
        }

        public async Task GuardarAsync(ManualVotosAsignacion asignacion)
        {
            var entity = new ManualVotosAsignacionEntity
            {
                Id = asignacion.Id,
                VotacionId = asignacion.VotacionId,
                ProyectoId = asignacion.ProyectoId,
                PosicionFinal = asignacion.PosicionFinal,
                VotosAsignados = asignacion.VotosAsignados,
                FechaCreacion = asignacion.FechaCreacion,
                CreadoPor = asignacion.CreadoPor,
                TextoJustificacion = asignacion.TextoJustificacion,
                UsuarioJustificacion = asignacion.UsuarioJustificacion,
                RolUsuarioJustificacion = asignacion.RolUsuarioJustificacion,
                FechaJustificacion = asignacion.FechaJustificacion
            };

            var existente = await _context.ManualVotosAsignaciones
                .FirstOrDefaultAsync(m => m.VotacionId == asignacion.VotacionId && m.ProyectoId == asignacion.ProyectoId);

            if (existente != null)
            {
                existente.PosicionFinal = asignacion.PosicionFinal;
                existente.VotosAsignados = asignacion.VotosAsignados;
                existente.FechaCreacion = DateTime.UtcNow;
                existente.TextoJustificacion = asignacion.TextoJustificacion;
                existente.UsuarioJustificacion = asignacion.UsuarioJustificacion;
                existente.RolUsuarioJustificacion = asignacion.RolUsuarioJustificacion;
                existente.FechaJustificacion = asignacion.FechaJustificacion;
                _context.ManualVotosAsignaciones.Update(existente);
            }
            else
            {
                _context.ManualVotosAsignaciones.Add(entity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ManualVotosAsignacion>> ObtenerPorVotacionAsync(Guid votacionId)
        {
            var entities = await _context.ManualVotosAsignaciones
                .Where(m => m.VotacionId == votacionId)
                .ToListAsync();

            return entities.Select(e => new ManualVotosAsignacion
            {
                Id = e.Id,
                VotacionId = e.VotacionId,
                ProyectoId = e.ProyectoId,
                PosicionFinal = e.PosicionFinal,
                VotosAsignados = e.VotosAsignados,
                FechaCreacion = e.FechaCreacion,
                CreadoPor = e.CreadoPor,
                TextoJustificacion = e.TextoJustificacion,
                UsuarioJustificacion = e.UsuarioJustificacion,
                RolUsuarioJustificacion = e.RolUsuarioJustificacion,
                FechaJustificacion = e.FechaJustificacion
            }).ToList();
        }

        public async Task<ManualVotosAsignacion?> ObtenerPorIdAsync(Guid id)
        {
            var entity = await _context.ManualVotosAsignaciones.FindAsync(id);
            if (entity == null) return null;

            return new ManualVotosAsignacion
            {
                Id = entity.Id,
                VotacionId = entity.VotacionId,
                ProyectoId = entity.ProyectoId,
                PosicionFinal = entity.PosicionFinal,
                VotosAsignados = entity.VotosAsignados,
                FechaCreacion = entity.FechaCreacion,
                CreadoPor = entity.CreadoPor,
                TextoJustificacion = entity.TextoJustificacion,
                UsuarioJustificacion = entity.UsuarioJustificacion,
                RolUsuarioJustificacion = entity.RolUsuarioJustificacion,
                FechaJustificacion = entity.FechaJustificacion
            };
        }

        public async Task EliminarAsync(Guid votacionId, Guid proyectoId)
        {
            var entity = await _context.ManualVotosAsignaciones
                .FirstOrDefaultAsync(m => m.VotacionId == votacionId && m.ProyectoId == proyectoId);

            if (entity != null)
            {
                _context.ManualVotosAsignaciones.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
