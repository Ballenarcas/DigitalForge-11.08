using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
    [Table("ManualVotosAsignacion")]
    public class ManualVotosAsignacionEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("votacion_id")]
        public Guid VotacionId { get; set; }

        [Required]
        [Column("proyecto_id")]
        public Guid ProyectoId { get; set; }

        [Required]
        [Column("posicion_final")]
        public int PosicionFinal { get; set; }

        [Required]
        [Column("votos_asignados")]
        public int VotosAsignados { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("creado_por")]
        public string? CreadoPor { get; set; }

        [Column("texto_justificacion")]
        public string? TextoJustificacion { get; set; }

        [Column("usuario_justificacion")]
        public string? UsuarioJustificacion { get; set; }

        [Column("rol_usuario_justificacion")]
        public string? RolUsuarioJustificacion { get; set; }

        [Column("fecha_justificacion")]
        public DateTime? FechaJustificacion { get; set; }
    }
}
