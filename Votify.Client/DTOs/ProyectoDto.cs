using System;

namespace Votify.Client.DTOs
{
    public class ProyectoDto
    {
        public string Id { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
        public string? Equipo_Id { get; set; }
        public string? EquipoNombre { get; set; }
        public Guid VotacionId { get; set; }
        public string? ImagenUrl { get; set; }
        public Guid? ParticipanteId { get; set; }
    }
}