using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Domain.Factory;
using Votify.Domain.Builders;
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

            var entity = await _context.Proyectos.FindAsync(guidId);
            if (entity == null)
            {
                return null;
            }
            return new ProyectoBuilder()
                .ConNombre(entity.Nombre)
                .ConDescripcion(entity.Descripcion)
                .DelEquipo(entity.Equipo_Id.ToString())
                .DeLaVotacion(entity.VotacionId)
                .ConImagen(entity.ImagenUrl)
                .ConId(entity.Id.ToString())
                .Build();
        }

        public async Task<List<Proyecto>> ObtenerTodasAsync()
        {
            var entities = await _context.Proyectos.ToListAsync();
            return entities.Select(p => new ProyectoBuilder()
                .ConNombre(p.Nombre)
                .ConDescripcion(p.Descripcion)
                .DelEquipo(p.Equipo_Id.ToString())
                .DeLaVotacion(p.VotacionId)
                .ConImagen(p.ImagenUrl)
                .ConId(p.Id.ToString())
                .Build()).ToList();
        }

        public async Task<List<Proyecto>> ObtenerPorVotacionAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid))
                return new List<Proyecto>();

            var entities = await _context.Proyectos
                .Where(p => p.VotacionId == votacionGuid)
                .ToListAsync();
            
            return entities.Select(p => new ProyectoBuilder()
                .ConNombre(p.Nombre)
                .ConDescripcion(p.Descripcion)
                .DelEquipo(p.Equipo_Id.ToString())
                .DeLaVotacion(p.VotacionId)
                .ConImagen(p.ImagenUrl)
                .ConId(p.Id.ToString())
                .Build()).ToList();
        }
    }
}