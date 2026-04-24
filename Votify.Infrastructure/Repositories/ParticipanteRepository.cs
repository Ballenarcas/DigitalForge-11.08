using System;
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

            return new Participante(entity.Nombre, entity.Email, entity.PasswordHash)
            {
                Id = entity.Id,
                Rol = entity.Rol
            };
        }

        public async Task AddAsync(Participante participante)
        {
            var entity = new ParticipanteEntity
            {
                Id = participante.Id,
                Nombre = participante.Nombre,
                Email = participante.Email,
                Rol = participante.Rol,
                PasswordHash = participante.PasswordHash
            };
            
            await _context.Participantes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
    }
}