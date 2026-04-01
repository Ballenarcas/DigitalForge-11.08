using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
    [Table("votos")]
    public class VotoEntity
    {
      [Key]
      [Column("id")]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public Guid Id { get; set; }
      [Column("proyecto_id")]
      public Guid ProyectoId { get; set; }
      [Column("votante_id")]
      public Guid? VotanteId { get; set; }
      [Column("votacion_id")]
      public Guid VotacionId { get; set; }
      [Column("fecha")]
      public DateTime Fecha { get; set; }
    }
}