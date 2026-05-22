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
        private readonly IStorageService _storageService;

        public VotacionRepository(VotifyDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
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
                Estado = votacion.Estado is Domain.Estado.EstadoActiva ? 0
                   : votacion.Estado is Domain.Estado.EstadoPausada ? 1
                   : 2,
                ImagenUrl = votacion.ImagenUrl
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
            entity.Estado = votacion.Estado is Domain.Estado.EstadoActiva ? 0
                         : votacion.Estado is Domain.Estado.EstadoPausada ? 1
                         : 2;
            entity.ImagenUrl = votacion.ImagenUrl;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return false;

            var entity = await _db.Votaciones.FindAsync(guid);
            if (entity is null) return false;

            if (!string.IsNullOrEmpty(entity.ImagenUrl))
            {
                try { await _storageService.EliminarArchivoAsync("Eventos", entity.ImagenUrl); }
                catch { }
            }

            var votos = await _db.Votos.Where(v => v.VotacionId == guid).ToListAsync();
            if (votos.Any())
            {
                _db.Votos.RemoveRange(votos);
            }

            var proyectos = await _db.Proyectos.Where(p => p.VotacionId == guid).ToListAsync();

            foreach (var proyecto in proyectos)
            {
                var comentarios = await _db.Comentarios.Where(c => c.Proyecto_Id == proyecto.Id).ToListAsync();
                if (comentarios.Any())
                    _db.Comentarios.RemoveRange(comentarios);

                var asignaciones = await _db.ManualVotosAsignaciones.Where(a => a.ProyectoId == proyecto.Id && a.VotacionId == guid).ToListAsync();
                if (asignaciones.Any())
                    _db.ManualVotosAsignaciones.RemoveRange(asignaciones);
            }

            if (proyectos.Any())
            {
                _db.Proyectos.RemoveRange(proyectos);
            }

            var asignacionesSueltas = await _db.ManualVotosAsignaciones.Where(a => a.VotacionId == guid).ToListAsync();
            if (asignacionesSueltas.Any())
                _db.ManualVotosAsignaciones.RemoveRange(asignacionesSueltas);

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
                "MULTICRITERIO_PUBLICO" => new VotacionMulticriterioPublicoFactory(),
                _ => new VotacionEstandarFactory()
            };

            var domain = factory.Crear(
                entity.Nombre,
                entity.FechaInicio,
                entity.FechaFin,
                entity.LimiteProy,
                entity.Comentarios,
                entity.ComentariosObligatorios,
                entity.EventoId,
                entity.EsAnonima,
                entity.ImagenUrl
            );
            domain.Id = entity.Id;

            var estadoInicial = entity.Estado switch
            {
                0 => (Domain.Estado.IEstadoVotacion)new Domain.Estado.EstadoActiva(),
                1 => new Domain.Estado.EstadoPausada(),
                2 => new Domain.Estado.EstadoFinalizada(),
                _ => (Domain.Estado.IEstadoVotacion)new Domain.Estado.EstadoActiva()
            };
            domain.CambiarEstado(estadoInicial);

            return domain;
        }

        public async Task ActualizarEstadoAsync(string id, Domain.Entities.EstadoVotacion estado)
        {
            if (!Guid.TryParse(id, out var guid)) 
                throw new ArgumentException("El ID no es válido.");

            var entity = await _db.Votaciones.FindAsync(guid);
            if (entity is null)
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");

            entity.Estado = estado switch
            {
                Domain.Entities.EstadoVotacion.Abierta => 0,
                Domain.Entities.EstadoVotacion.Pausada => 1,
                Domain.Entities.EstadoVotacion.Detenida => 2,
                _ => 0
            };
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
