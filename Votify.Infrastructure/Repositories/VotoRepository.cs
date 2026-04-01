using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Domain.Factory;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Votify.Infrastructure.Repositories
{
    public class VotoRepository : IVotoRepository
    {
        private readonly VotifyDbContext _db;

        public VotoRepository(VotifyDbContext db)
        {
            _db = db;
        }

        public async Task GuardarAsync(Voto voto)
        {
            var entity = new VotoEntity
            {
                Id = Guid.NewGuid(), // Evitar depender de la auto-generación del Guid en PostgreSQL
                ProyectoId = Guid.Parse(voto.ProyectoId),
                VotacionId = Guid.Parse(voto.VotacionId),
                // Si es un voto anónimo (votante nulo), pasamos null a la BD para no violar la Foreign Key
                VotanteId = string.IsNullOrEmpty(voto.VotanteId) ? (Guid?)null : Guid.Parse(voto.VotanteId),
                Fecha = DateTime.UtcNow
            };

            await _db.Votos.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Voto>> ObtenerPorProyectoAsync(string proyectoId)
        {
            if (!Guid.TryParse(proyectoId, out var guid)) return new List<Voto>();

            var entities = await _db.Votos.Where(v => v.ProyectoId == guid).ToListAsync();
            return entities.Select(MapToDomain).ToList();
        }

        public async Task<int> ContarVotosPorUsuarioYVotacionAsync(string votacionId, string votanteId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid) || !Guid.TryParse(votanteId, out var votanteGuid))
                return 0;

            return await _db.Votos
                .CountAsync(v => v.VotacionId == votacionGuid && v.VotanteId == votanteGuid);
        }

        private Voto MapToDomain(VotoEntity entity)
        {
            VotoFactory factory;
            
            // Si el votante es null asumimos que fue un voto anónimo
            if (entity.VotanteId == null || entity.VotanteId == Guid.Empty)
            {
                factory = new VotoAnonimoFactory();
            }
            else
            {
                factory = new VotoEstandarFactory();
            }

            var votanteStr = (entity.VotanteId == null || entity.VotanteId == Guid.Empty) ? null : entity.VotanteId.ToString();
            
            return factory.Crear(entity.ProyectoId.ToString(), entity.VotacionId.ToString(), votanteStr);
        }
    }
}