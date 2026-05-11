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
    public class EquipoRepository : IEquipoRepository
    {
        private readonly VotifyDbContext _context;

        public EquipoRepository(VotifyDbContext context)
        {
            _context = context;
        }

        public async Task GuardarAsync(Equipo equipo)
        {
            var entity = new EquipoEntity
            {
                Id = equipo.Id,
                Nombre = equipo.Nombre,
                CreatedAt = equipo.CreatedAt
            };

            await _context.Equipos.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Equipo?> ObtenerPorIdAsync(Guid id)
        {
            var entity = await _context.Equipos.FindAsync(id);
            if (entity == null) return null;

            return new Equipo(entity.Nombre, entity.Id, entity.CreatedAt);
        }

        public async Task<IEnumerable<Equipo>> ObtenerTodosAsync()
        {
            var entities = await _context.Equipos.ToListAsync();
            return entities.Select(e => new Equipo(e.Nombre, e.Id, e.CreatedAt));
        }
    }
}
