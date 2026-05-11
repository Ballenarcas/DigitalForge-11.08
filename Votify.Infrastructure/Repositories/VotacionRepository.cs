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
                Tipo = votacion.Tipo,
                FechaInicio = votacion.FechaInicio.ToUniversalTime(),
                FechaFin = votacion.FechaFin.ToUniversalTime(),
                LimiteProy = votacion.LimiteProy,
                Comentarios = votacion.Comentarios,
                ComentariosObligatorios = votacion.ComentariosObligatorios,
                EsAnonima = votacion.EsAnonima,
                EventoId = votacion.EventoId,
                Estado = (int)votacion.Estado
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

        public async Task<List<Votacion>> ObtenerPorEventoAsync(Guid eventoId)
        {
            var entities = await _db.Votaciones
                .Where(v => v.EventoId == eventoId)
                .ToListAsync();
            return entities.Select(MapToDomain).ToList();
        }

        public async Task<bool> ActualizarAsync(string id, Votacion votacion)
        {
            if (!Guid.TryParse(id, out var guid)) return false;

            var entity = await _db.Votaciones.FindAsync(guid);
            if (entity is null) return false;

            entity.Nombre = votacion.Nombre;
            entity.Tipo = votacion.Tipo;
            entity.FechaInicio = votacion.FechaInicio.ToUniversalTime();
            entity.FechaFin = votacion.FechaFin.ToUniversalTime();
            entity.LimiteProy = votacion.LimiteProy;
            entity.Comentarios = votacion.Comentarios;
            entity.ComentariosObligatorios = votacion.ComentariosObligatorios;
            entity.EsAnonima = votacion.EsAnonima;
            entity.EventoId = votacion.EventoId;
            entity.Estado = (int)votacion.Estado;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return false;

            var entity = await _db.Votaciones.FindAsync(guid);
            if (entity is null) return false;

            // Eliminar votos asociados a la votación
            var votos = await _db.Votos.Where(v => v.VotacionId == guid).ToListAsync();
            if (votos.Any())
            {
                _db.Votos.RemoveRange(votos);
            }

            // Eliminar proyectos asociados a la votación
            var proyectos = await _db.Proyectos.Where(p => p.VotacionId == guid).ToListAsync();
            if (proyectos.Any())
            {
                _db.Proyectos.RemoveRange(proyectos);
            }

            // Finalmente, eliminar la votación
            _db.Votaciones.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        private Votacion MapToDomain(VotacionEntity entity)
        {
            var tipoNormalized = entity.Tipo?.ToUpper() ?? "ESTANDAR";
            
            // Retrocompatibilidad: tratar datos antiguos de base de datos como ESTANDAR
            if (tipoNormalized == "RECUENTO DE VOTOS")
            {
                tipoNormalized = "ESTANDAR";
            }

            VotacionFactory factory = tipoNormalized switch
            {
                "ESTANDAR" => new VotacionEstandarFactory(),
                "MULTICRITERIO" => new VotacionMulticriterioFactory(),
                _ => throw new Exception($"Tipo de votación desconocido en la base de datos: {entity.Tipo}")
            };

            var domain = factory.Crear(
                entity.Nombre,
                entity.FechaInicio,
                entity.FechaFin,
                entity.LimiteProy,
                entity.Comentarios,
                entity.ComentariosObligatorios,
                entity.EventoId,
                entity.EsAnonima
            );
            domain.Id = entity.Id;
            domain.Estado = (Domain.Entities.EstadoVotacion)entity.Estado;
            return domain;
        }

        public async Task ActualizarEstadoAsync(string id, Domain.Entities.EstadoVotacion estado)
        {
            if (!Guid.TryParse(id, out var guid)) 
                throw new ArgumentException("El ID no es válido.");

            var entity = await _db.Votaciones.FindAsync(guid);
            if (entity is null)
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");

            entity.Estado = (int)estado;
            await _db.SaveChangesAsync();
        }

        public async Task<string?> ObtenerEventoIdAsync(string votacionId)
        {
            if (!Guid.TryParse(votacionId, out var guid)) return null;

            var entity = await _db.Votaciones.FindAsync(guid);
            return entity?.EventoId.ToString();
        }
    }
}
