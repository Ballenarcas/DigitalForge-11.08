using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
    [Table("Comentario")]
    public class ComentarioEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("proyecto_id")]
        public Guid Proyecto_Id { get; set; }

        [Column("autor_id")]
        public Guid? Autor_Id { get; set; }

        [Required]
        [Column("texto")]
        public string Texto { get; set; } = string.Empty;
        
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
