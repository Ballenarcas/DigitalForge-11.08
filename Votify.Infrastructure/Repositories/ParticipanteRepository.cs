using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;

namespace Votify.Infrastructure.Repositories
{
    public class ParticipanteRepository : IParticipanteRepository
    {
        private readonly VotifyDbContext _context;
        
        public ParticipanteRepository(VotifyDbContext context) 
        { 
            _context = context; 
        }

        public async Task<Participante?> GetByEmailAsync(string email)
        {
            var entity = await _context.Participantes.FirstOrDefaultAsync(p => p.Email == email);
            if (entity == null)
            {
                return null;
            }

            return new Participante(entity.Nombre, entity.Email, entity.PasswordHash, entity.EquipoId)
            {
                Id = entity.Id
            };
        }

        public async Task<Participante?> ObtenerPorIdAsync(Guid id)
        {
            var entity = await _context.Participantes.FindAsync(id);
            if (entity == null) return null;

            return new Participante(entity.Nombre, entity.Email, entity.PasswordHash, entity.EquipoId)
            {
                Id = entity.Id
            };
        }

        public async Task AddAsync(Participante participante)
        {
            var entity = new ParticipanteEntity
            {
                Id = participante.Id,
                Nombre = participante.Nombre,
                Email = participante.Email,
                PasswordHash = participante.PasswordHash,
                EquipoId = participante.EquipoId
            };
            
            await _context.Participantes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Participante participante)
        {
            var entity = await _context.Participantes.FindAsync(participante.Id);
            if (entity != null)
            {
                entity.Nombre = participante.Nombre;
                entity.Email = participante.Email;
                entity.PasswordHash = participante.PasswordHash;
                entity.EquipoId = participante.EquipoId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Participante>> ObtenerTodosAsync()
        {
            var entities = await _context.Participantes.ToListAsync();
            return entities.Select(e => new Participante(e.Nombre, e.Email, e.PasswordHash, e.EquipoId)
            {
                Id = e.Id
            });
        }
    }
}