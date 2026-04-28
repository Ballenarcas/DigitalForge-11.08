using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
[Table("votacion")] 
public class VotacionEntity
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } 

    [Column("nombre")]
    public string Nombre { get; set; } = default!;

    [Column("tipo")]
    public string Tipo { get; set; } = default!;

    [Column("fecha_inicio")]
    public DateTime FechaInicio { get; set; }

    [Column("fecha_fin")]
    public DateTime FechaFin { get; set; }

    [Column("limite_proy")]
    public int LimiteProy { get; set; }

    [Column("comentarios")]
    public bool Comentarios { get; set; }

    [Column("EsAnonima")]
    public bool EsAnonima { get; set; }

    [Column("evento")]
    public Guid EventoId { get; set; }
}
}