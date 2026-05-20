using System.ComponentModel.DataAnnotations;

namespace Votify.Application.DTOs;

public class ProyectoDto
{
    public required string Id { get; set; }
    public required string Nombre { get; set; }
    public required string Descripcion { get; set; }
    public string? Equipo_Id { get; set; }
    public string? EquipoNombre { get; set; }
    public Guid VotacionId { get; set; }
    public string? ImagenUrl { get; set; }
    public Guid? ParticipanteId { get; set; }
}
