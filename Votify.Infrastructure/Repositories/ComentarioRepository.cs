using Microsoft.EntityFrameworkCore;
using Votify.Domain.Entities;
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

        public async Task GuardarAsync(string proyectoId, string texto, Guid? autorId = null)
        {
            if (!Guid.TryParse(proyectoId, out var guidProyectoId))
            {
                throw new ArgumentException("El ID del proyecto no es válido.");
            }

            var entity = new ComentarioEntity
            {
                Proyecto_Id = guidProyectoId,
                Autor_Id = autorId,
                Texto = texto,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Comentarios.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Comentario>> ObtenerAsync(string proyectoId)
        {
            if (!Guid.TryParse(proyectoId, out var guidProyectoId))
            {
                return new List<Comentario>();
            }

            var comentarios = await _context.Comentarios
                .Where(c => c.Proyecto_Id == guidProyectoId)
                .OrderByDescending(c => c.FechaCreacion)
                .Select(c => new Comentario
                {
                    Texto = c.Texto,
                    AutorId = c.Autor_Id,
                    FechaCreacion = c.FechaCreacion
                })
                .ToListAsync();

            return comentarios;
        }

        public async Task<bool> HaComentadoProyectoAsync(string proyectoId, Guid autorId)
        {
            if (!Guid.TryParse(proyectoId, out var guidProyectoId))
            {
                return false;
            }

            return await _context.Comentarios
                .AnyAsync(c => c.Proyecto_Id == guidProyectoId && c.Autor_Id == autorId);
        }
    }
}
