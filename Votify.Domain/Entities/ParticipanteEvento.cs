using System;

namespace Votify.Domain.Entities
{
    public class ParticipanteEvento
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ParticipanteId { get; set; }
        public Guid EventoId { get; set; }
        public string Rol { get; set; }

        public ParticipanteEvento(Guid participanteId, Guid eventoId, string rol)
        {
            ParticipanteId = participanteId;
            EventoId = eventoId;
            Rol = rol;
        }
    }
}