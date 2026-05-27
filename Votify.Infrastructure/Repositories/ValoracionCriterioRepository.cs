using Microsoft.EntityFrameworkCore;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;

namespace Votify.Infrastructure.Repositories
{
    public class ValoracionCriterioRepository : IValoracionCriterioRepository
    {
        private readonly VotifyDbContext _db;

        public ValoracionCriterioRepository(VotifyDbContext db)
        {
            _db = db;
        }

        public async Task<bool> HaValoradoProyectoAsync(string proyectoId, string votanteId)
        {
            if (!Guid.TryParse(proyectoId, out var proyectoGuid) || !Guid.TryParse(votanteId, out var votanteGuid))
            {
                return false;
            }

            return await _db.ValoracionesCriterio.AnyAsync(v => v.ProyectoId == proyectoGuid && v.VotanteId == votanteGuid);
        }

        public async Task GuardarAsync(string proyectoId, string votanteId, List<ValoracionCriterio> valoraciones)
        {
            if (!Guid.TryParse(proyectoId, out var proyectoGuid) || !Guid.TryParse(votanteId, out var votanteGuid))
            {
                throw new ArgumentException("Proyecto o votante no válido.");
            }

            var entities = valoraciones.Select(v => new ValoracionCriterioEntity
            {
                ProyectoId = proyectoGuid,
                VotanteId = votanteGuid,
                CriterioId = v.CriterioId,
                Valoracion = v.Valoracion
            });

            await _db.ValoracionesCriterio.AddRangeAsync(entities);
            await _db.SaveChangesAsync();
        }

        public async Task<List<ValoracionCriterio>> ObtenerPorProyectoYVotanteAsync(string proyectoId, string votanteId)
        {
            if (!Guid.TryParse(proyectoId, out var proyectoGuid) || !Guid.TryParse(votanteId, out var votanteGuid))
            {
                return new List<ValoracionCriterio>();
            }

            var entities = await _db.ValoracionesCriterio
                .AsNoTracking()
                .Where(v => v.ProyectoId == proyectoGuid && v.VotanteId == votanteGuid)
                .ToListAsync();

            return entities.Select(v => new ValoracionCriterio
            {
                Id = v.Id,
                ProyectoId = v.ProyectoId,
                VotanteId = v.VotanteId,
                CriterioId = v.CriterioId,
                Valoracion = v.Valoracion
            }).ToList();
        }

        public async Task<List<(string ProyectoId, double Puntaje, int Evaluaciones)>> ObtenerResultadosPonderadosAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid))
            {
                return new List<(string, double, int)>();
            }

            var rows = await (
                from valoracion in _db.ValoracionesCriterio
                join criterio in _db.Criterios on valoracion.CriterioId equals criterio.Id
                where criterio.VotacionId == votacionGuid
                group new { valoracion, criterio } by valoracion.ProyectoId into g
                select new
                {
                    ProyectoId = g.Key,
                    Puntaje = g.Sum(x => (double)x.valoracion.Valoracion * ((double)x.criterio.Peso / 100.0)) / g.Select(x => x.valoracion.VotanteId).Distinct().Count(),
                    Evaluaciones = g.Select(x => x.valoracion.VotanteId).Distinct().Count()
                })
                .OrderByDescending(x => x.Puntaje)
                .ToListAsync();

            return rows.Select(r => (r.ProyectoId.ToString(), r.Puntaje, r.Evaluaciones)).ToList();
        }

        public async Task<List<(string ProyectoId, Guid CriterioId, double PromedioValoracion, int NumEvaluaciones)>> ObtenerDetallesPorCriterioAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid))
            {
                return new List<(string, Guid, double, int)>();
            }

            var rows = await (
                from valoracion in _db.ValoracionesCriterio
                join criterio in _db.Criterios on valoracion.CriterioId equals criterio.Id
                where criterio.VotacionId == votacionGuid
                group valoracion by new { valoracion.ProyectoId, valoracion.CriterioId } into g
                select new
                {
                    ProyectoId = g.Key.ProyectoId,
                    CriterioId = g.Key.CriterioId,
                    PromedioValoracion = g.Average(x => (double)x.Valoracion),
                    NumEvaluaciones = g.Count()
                })
                .ToListAsync();

            return rows.Select(r => (r.ProyectoId.ToString(), r.CriterioId, r.PromedioValoracion, r.NumEvaluaciones)).ToList();
        }

        public async Task EliminarPorVotacionAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var votacionGuid)) return;

            var criterioIds = await _db.Criterios.Where(c => c.VotacionId == votacionGuid).Select(c => c.Id).ToListAsync();
            var valoraciones = await _db.ValoracionesCriterio.Where(v => criterioIds.Contains(v.CriterioId)).ToListAsync();
            if (valoraciones.Any())
            {
                _db.ValoracionesCriterio.RemoveRange(valoraciones);
                await _db.SaveChangesAsync();
            }
        }
    }
}
