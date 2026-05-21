using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
    [Table("notificacion")]
    public class NotificacionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("usuario_id")]
        public Guid UsuarioId { get; set; }

        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [Column("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Column("recurso_id")]
        public string? RecursoId { get; set; }

        [Column("recurso_tipo")]
        public string? RecursoTipo { get; set; }

        [Column("leida")]
        public bool Leida { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}