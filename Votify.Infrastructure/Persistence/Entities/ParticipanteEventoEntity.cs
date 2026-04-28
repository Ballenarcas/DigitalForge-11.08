using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
    [Table("participante_evento")]
    public class ParticipanteEventoEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("participante_id")]
        public Guid ParticipanteId { get; set; }

        [Column("evento_id")]
        public Guid EventoId { get; set; }

        [Column("rol")]
        public string Rol { get; set; } = string.Empty;
    }
}