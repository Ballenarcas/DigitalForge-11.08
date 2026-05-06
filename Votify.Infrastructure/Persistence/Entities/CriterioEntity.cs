using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
    [Table("criterio")]
    public class CriterioEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("votacion_id")]
        public Guid VotacionId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("tipo")]
        public string Tipo { get; set; } = "Estrellas";

        [Column("peso")]
        public decimal Peso { get; set; }
    }
}
