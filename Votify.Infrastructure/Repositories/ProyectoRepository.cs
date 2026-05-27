using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
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
            if (!Guid.TryParse(proyecto.Equipo_Id, out var equipoId))
            {
                throw new ArgumentException("El equipo del proyecto no es válido.");
            }

            var entity = new ProyectoEntity
            {
                Id = Guid.Parse(proyecto.Id),
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion,
                Equipo_Id = equipoId,
                VotacionId = proyecto.VotacionId,
                ImagenUrl = proyecto.ImagenUrl
            };
            _context.Proyectos.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Proyecto?> ObtenerAsync(string proyectoId)
        {
            if (!Guid.TryParse(proyectoId, out var guidId)) return null;

            var entity = await _context.Proyectos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == guidId);
            if (entity == null)
            {
                return null;
            }
            return new Proyecto(
                entity.Nombre,
                entity.Descripcion,
                entity.Equipo_Id.ToString(),
                entity.VotacionId,
                entity.ImagenUrl,
                entity.Id.ToString());
        }

        public async Task<List<Proyecto>> ObtenerTodasAsync()
        {
            var entities = await _context.Proyectos.AsNoTracking().ToListAsync();
            return entities.Select(p => new Proyecto(
                p.Nombre,
                p.Descripcion,
                p.Equipo_Id.ToString(),
                p.VotacionId,
                p.ImagenUrl,
                p.Id.ToString())).ToList();
        }

        public async Task<List<Proyecto>> ObtenerPorVotacionAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid))
                return new List<Proyecto>();

            var entities = await _context.Proyectos
                .AsNoTracking()
                .Where(p => p.VotacionId == votacionGuid)
                .ToListAsync();

            return entities.Select(p => new Proyecto(
                p.Nombre,
                p.Descripcion,
                p.Equipo_Id.ToString(),
                p.VotacionId,
                p.ImagenUrl,
                p.Id.ToString())).ToList();
        }
    }
}