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
                Id = Guid.NewGuid(),
                ProyectoId = Guid.Parse(voto.ProyectoId),
                VotacionId = Guid.Parse(voto.VotacionId),
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

        public async Task<List<(string ProyectoId, int Votos)>> ObtenerVotosPorVotacionAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid))
                return new List<(string, int)>();

            var resultado = await _db.Votos
                .Where(v => v.VotacionId == votacionGuid)
                .GroupBy(v => v.ProyectoId)
                .Select(g => new { ProyectoId = g.Key, Votos = g.Count() })
                .OrderByDescending(x => x.Votos)
                .ToListAsync();

            return resultado.Select(x => (x.ProyectoId.ToString(), x.Votos)).ToList();
        }

        private Voto MapToDomain(VotoEntity entity)
        {
            VotoFactory factory;
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

        public async Task<bool> EliminarPorVotacionAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid)) return false;

            var votos = await _db.Votos.Where(v => v.VotacionId == votacionGuid).ToListAsync();
            
            if (votos.Any())
            {
                _db.Votos.RemoveRange(votos);
                await _db.SaveChangesAsync();
            }

            return true;
        }
    }
}