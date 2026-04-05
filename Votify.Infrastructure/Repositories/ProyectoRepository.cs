using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Domain.Factory;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Votify.Infrastructure.Repositories
{
    public class ProyectoRepository : IProyectoRepository
    {
        private readonly VotifyDbContext _context;

        public ProyectoRepository(VotifyDbContext context)
        {
            _context = context;
        }

        public async Task GuardarAsync(Proyecto proyecto)
        {
            var entity = new ProyectoEntity
            {
                Categoria_Id = string.IsNullOrEmpty(proyecto.Categoria_Id) ? null : Guid.Parse(proyecto.Categoria_Id),
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion,
                Equipo_Id = string.IsNullOrEmpty(proyecto.Equipo_Id) ? null : Guid.Parse(proyecto.Equipo_Id)
            };
            _context.Proyectos.Add(entity);
            await _context.SaveChangesAsync();
            proyecto.Id = entity.Id.ToString();
        }

        public async Task<Proyecto?> ObtenerAsync(string proyectoId)
        {
            if (!Guid.TryParse(proyectoId, out var guidId)) return null;

            var entity = await _context.Proyectos.FindAsync(guidId);
            if (entity == null)
            {
                return null;
            }
            return new Proyecto(
                entity.Categoria_Id?.ToString(), 
                entity.Nombre, 
                entity.Descripcion, 
                entity.Equipo_Id?.ToString(), 
                entity.Id.ToString());
        }

        public async Task<List<Proyecto>> ObtenerTodasAsync()
        {
            var entities = await _context.Proyectos.ToListAsync();
            return entities.Select(p => new Proyecto(
                p.Categoria_Id?.ToString(), 
                p.Nombre, 
                p.Descripcion, 
                p.Equipo_Id?.ToString(), 
                p.Id.ToString())).ToList();
        }

        public async Task<List<Proyecto>> ObtenerPorVotacionAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid))
                return new List<Proyecto>();

            var proyectoIds = await _context.Votos
                .Where(v => v.VotacionId == votacionGuid)
                .Select(v => v.ProyectoId)
                .Distinct()
                .ToListAsync();

            var entities = await _context.Proyectos
                .Where(p => proyectoIds.Contains(p.Id))
                .ToListAsync();

            return entities.Select(p => new Proyecto(
                p.Categoria_Id?.ToString(), 
                p.Nombre, 
                p.Descripcion, 
                p.Equipo_Id?.ToString(), 
                p.Id.ToString())).ToList();
        }
    }
}