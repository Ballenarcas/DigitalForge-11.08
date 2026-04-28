using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
    [Table("evento")]
    public class EventoEntity
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = default!;

        [Column("descripcion")]
        public string Descripcion { get; set; } = default!;

        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime FechaFin { get; set; }

        [Column("imagen_url")]
        public string? ImagenUrl { get; set; }
    }
}
