using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Domain.Factory;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Votify.Infrastructure.Repositories
{
    public class VotacionRepository : IVotacionRepository
    {
        private readonly VotifyDbContext _db;

        public VotacionRepository(VotifyDbContext db)
        {
            _db = db;
        }

        public async Task GuardarAsync(Votacion votacion)
        {
            var entity = new VotacionEntity
            {
                Id = votacion.Id,
                Nombre = votacion.Nombre,
                Tipo = votacion.Tipo(),
                FechaInicio = votacion.FechaInicio.ToUniversalTime(),
                FechaFin = votacion.FechaFin.ToUniversalTime(),
                LimiteProyectos = votacion.LimiteProyectos,
                PermiteComentarios = votacion.PermiteComentarios
            };

            await _db.Votaciones.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<Votacion?> ObtenerAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return null;
            
            var entity = await _db.Votaciones.FindAsync(guid);
            return entity == null ? null : MapToDomain(entity);
        }

        public async Task<List<Votacion>> ObtenerTodasAsync()
        {
            var entities = await _db.Votaciones.ToListAsync();
            return entities.Select(MapToDomain).ToList();
        }

        public async Task<bool> ActualizarAsync(string id, Votacion votacion)
        {
            if (!Guid.TryParse(id, out var guid)) return false;

            var entity = await _db.Votaciones.FindAsync(guid);
            if (entity is null) return false;

            entity.Nombre = votacion.Nombre;
            entity.Tipo = votacion.Tipo();
            entity.FechaInicio = votacion.FechaInicio.ToUniversalTime();
            entity.FechaFin = votacion.FechaFin.ToUniversalTime();
            entity.LimiteProyectos = votacion.LimiteProyectos;
            entity.PermiteComentarios = votacion.PermiteComentarios;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return false;

            var entity = await _db.Votaciones.FindAsync(guid);
            if (entity is null) return false;

            var votos = await _db.Votos.Where(v => v.VotacionId == guid).ToListAsync();
            if (votos.Any())
            {
                _db.Votos.RemoveRange(votos);
            }

            _db.Votaciones.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        private Votacion MapToDomain(VotacionEntity entity)
        {
            VotacionFactory factory = entity.Tipo.ToUpper() switch
            {
                "ESTANDAR" => new VotacionEstandarFactory(),
                "ANONIMA" => new VotacionAnonimaFactory(),
                _ => throw new Exception("Tipo desconocido")
            };

            var domain = factory.Crear(
                entity.Nombre,
                entity.FechaInicio,
                entity.FechaFin,
                entity.LimiteProyectos,
                entity.PermiteComentarios
            );
            domain.Id = entity.Id;
            return domain;
        }
    }
}
