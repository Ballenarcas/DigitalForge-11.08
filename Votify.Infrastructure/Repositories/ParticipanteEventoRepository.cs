using System.Threading.Tasks;
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
            var pe = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                _db.ParticipantesEventos, 
                x => x.EventoId == eventoId && x.ParticipanteId == participanteId);
            return pe?.Rol;
        }
    }
}