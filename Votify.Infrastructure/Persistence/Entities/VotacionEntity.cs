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
    public int LimiteProyectos { get; set; }

    [Column("comentarios")]
    public bool PermiteComentarios { get; set; }
}
}