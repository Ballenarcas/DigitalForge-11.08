using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;

namespace Votify.Infrastructure.Repositories
{
    public class CriterioRepository : ICriterioRepository
    {
        private readonly VotifyDbContext _db;

        public CriterioRepository(VotifyDbContext db)
        {
            _db = db;
        }

        public async Task<List<Criterio>> ObtenerPorVotacionAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid))
            {
                return new List<Criterio>();
            }

            var entities = await _db.Criterios
                .AsNoTracking()
                .Where(c => c.VotacionId == votacionGuid)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return entities.Select(MapToDomain).ToList();
        }

        public async Task<Dictionary<string, List<Criterio>>> ObtenerPorVotacionesAsync(IEnumerable<string> votacionIds)
        {
            var guidIds = votacionIds
                .Where(id => Guid.TryParse(id, out _))
                .Select(Guid.Parse)
                .ToList();

            if (!guidIds.Any())
                return new Dictionary<string, List<Criterio>>();

            var entities = await _db.Criterios
                .AsNoTracking()
                .Where(c => guidIds.Contains(c.VotacionId))
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return entities
                .GroupBy(c => c.VotacionId.ToString())
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(MapToDomain).ToList()
                );
        }

        public async Task ReemplazarPorVotacionAsync(string votacionId, List<Criterio> criterios)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid))
            {
                throw new ArgumentException("El ID de la votación no es válido.");
            }

            var existentes = await _db.Criterios.Where(c => c.VotacionId == votacionGuid).ToListAsync();
            if (existentes.Any())
            {
                _db.Criterios.RemoveRange(existentes);
            }

            var nuevos = criterios.Select(c => new CriterioEntity
            {
                Id = c.Id == Guid.Empty ? Guid.NewGuid() : c.Id,
                VotacionId = votacionGuid,
                Nombre = c.Nombre,
                Tipo = string.IsNullOrWhiteSpace(c.Tipo) ? "Estrellas" : c.Tipo,
                Peso = c.Peso
            });

            await _db.Criterios.AddRangeAsync(nuevos);
            await _db.SaveChangesAsync();
        }

        public async Task EliminarPorVotacionAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid)) return;

            var criterios = await _db.Criterios.Where(c => c.VotacionId == votacionGuid).ToListAsync();
            if (criterios.Any())
            {
                _db.Criterios.RemoveRange(criterios);
                await _db.SaveChangesAsync();
            }
        }

        private static Criterio MapToDomain(CriterioEntity entity)
        {
            return new Criterio
            {
                Id = entity.Id,
                VotacionId = entity.VotacionId,
                Nombre = entity.Nombre,
                Tipo = entity.Tipo,
                Peso = entity.Peso
            };
        }
    }
}
