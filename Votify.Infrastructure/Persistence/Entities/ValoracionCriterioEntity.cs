using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
    [Table("valoracion_criterio")]
    public class ValoracionCriterioEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("votante_id")]
        public Guid VotanteId { get; set; }

        [Column("criterio_id")]
        public Guid CriterioId { get; set; }

        [Column("proyecto_id")]
        public Guid ProyectoId { get; set; }

        [Column("valoracion")]
        public int Valoracion { get; set; }
    }
}
