using Microsoft.EntityFrameworkCore;
using Votify.Domain.Interfaces;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;

namespace Votify.Infrastructure.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly VotifyDbContext _context;

        public ComentarioRepository(VotifyDbContext context)
        {
            _context = context;
        }

        public async Task GuardarAsync(string proyectoId, string texto)
        {
            if (!Guid.TryParse(proyectoId, out var guidProyectoId))
            {
                throw new ArgumentException("El ID del proyecto no es válido.");
            }

            var entity = new ComentarioEntity
            {
                Proyecto_Id = guidProyectoId,
                Texto = texto,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Comentarios.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> ObtenerAsync(string proyectoId)
        {
            if (!Guid.TryParse(proyectoId, out var guidProyectoId))
            {
                return new List<string>();
            }

            var comentarios = await _context.Comentarios
                .Where(c => c.Proyecto_Id == guidProyectoId)
                .OrderByDescending(c => c.FechaCreacion)
                .Select(c => c.Texto)
                .ToListAsync();

            return comentarios;
        }
    }
}
