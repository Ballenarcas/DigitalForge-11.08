using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
[Table("proyecto")] 
public class ProyectoEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } 

    [Column("nombre")]
    public string Nombre { get; set; } = default!;

    [Column("descripcion")]
    public string Descripcion { get; set; } = default!;

    [Column("equipo")]
    public Guid Equipo_Id { get; set; }

    [Column("votacion_id")]
    public Guid VotacionId { get; set; }

    [Column("imagen_url")]
    public string? ImagenUrl { get; set; }
}
}