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
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } 

    [Column("categoria_id")]
    public Guid? Categoria_Id { get; set; } 

    [Column("nombre")]
    public string Nombre { get; set; } = default!;

    [Column("descripcion")]
    public string Descripcion { get; set; } = default!;

    [Column("equipo")]
    public string? Equipo_Id { get; set; }
}
}