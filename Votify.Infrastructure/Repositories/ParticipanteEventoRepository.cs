using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Votify.Infrastructure.Persistence;
using Votify.Infrastructure.Persistence.Entities;

namespace Votify.Infrastructure.Repositories
{
    public class ParticipanteEventoRepository : IParticipanteEventoRepository
    {
        private readonly VotifyDbContext _db;

        public ParticipanteEventoRepository(VotifyDbContext db)
        {
            _db = db;
        }

        public async Task GuardarAsync(ParticipanteEvento participanteEvento)
        {
            var entity = new ParticipanteEventoEntity
            {
                Id = participanteEvento.Id,
                ParticipanteId = participanteEvento.ParticipanteId,
                EventoId = participanteEvento.EventoId,
                Rol = participanteEvento.Rol
            };

            await _db.ParticipantesEventos.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<string?> ObtenerRolAsync(Guid eventoId, Guid participanteId)
        {
            var pe = await _db.ParticipantesEventos
                .FirstOrDefaultAsync(x => x.EventoId == eventoId && x.ParticipanteId == participanteId);
            return pe?.Rol;
        }

        public async Task<List<ParticipanteEventoDetalle>> ObtenerParticipantesPorEventoAsync(Guid eventoId, string? search = null)
        {
            var query = from pe in _db.ParticipantesEventos
                        join p in _db.Participantes on pe.ParticipanteId equals p.Id
                        where pe.EventoId == eventoId
                        select new ParticipanteEventoDetalle
                        {
                            ParticipanteId = p.Id,
                            Nombre = p.Nombre,
                            Email = p.Email,
                            Rol = pe.Rol
                        };

            if (!string.IsNullOrWhiteSpace(search))
            {
                var likePattern = $"%{search}%";
                query = query.Where(x => EF.Functions.ILike(x.Nombre, likePattern) || EF.Functions.ILike(x.Email, likePattern));
            }

            return await query.OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<RoleStatistics> ContarRolesPorEventoAsync(Guid eventoId)
        {
            var group = await _db.ParticipantesEventos
                .Where(pe => pe.EventoId == eventoId)
                .GroupBy(pe => pe.Rol)
                .Select(g => new { Rol = g.Key, Count = g.Count() })
                .ToListAsync();

            var stats = new RoleStatistics();
            foreach (var item in group)
            {
                var rol = item.Rol?.Trim().ToUpperInvariant();
                if (rol == "ORGANIZADOR") stats.Organizadores = item.Count;
                else if (rol == "JURADO") stats.Jurados = item.Count;
                else if (rol == "COMPETIDOR") stats.Competidores = item.Count;
                else stats.Publicos += item.Count;
            }

            return stats;
        }

        public async Task<int> ContarParticipantesConRolAsync(Guid eventoId)
        {
            return await _db.ParticipantesEventos
                .CountAsync(pe => pe.EventoId == eventoId && !string.IsNullOrWhiteSpace(pe.Rol));
        }

        public async Task<bool> ActualizarRolAsync(Guid eventoId, Guid participanteId, string rol)
        {
            var pe = await _db.ParticipantesEventos
                .FirstOrDefaultAsync(x => x.EventoId == eventoId && x.ParticipanteId == participanteId);
            if (pe is null) return false;

            pe.Rol = rol;
            _db.ParticipantesEventos.Update(pe);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(Guid eventoId, Guid participanteId)
        {
            var pe = await _db.ParticipantesEventos
                .FirstOrDefaultAsync(x => x.EventoId == eventoId && x.ParticipanteId == participanteId);
            if (pe is null) return false;

            _db.ParticipantesEventos.Remove(pe);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Guid>> ObtenerOrganizadoresIdsAsync(Guid eventoId)
        {
            return await _db.ParticipantesEventos
                .Where(pe => pe.EventoId == eventoId && pe.Rol == "ORGANIZADOR")
                .Select(pe => pe.ParticipanteId)
                .ToListAsync();
        }
    }
}
