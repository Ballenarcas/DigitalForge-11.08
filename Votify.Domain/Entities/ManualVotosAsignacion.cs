namespace Votify.Domain.Entities
{
    public class ManualVotosAsignacion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VotacionId { get; set; }
        public Guid ProyectoId { get; set; }
        public int PosicionFinal { get; set; }
        public int VotosAsignados { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public string? CreadoPor { get; set; }

        // Justificación como comentario único
        public string? TextoJustificacion { get; set; }
        public string? UsuarioJustificacion { get; set; }
        public string? RolUsuarioJustificacion { get; set; }
        public DateTime? FechaJustificacion { get; set; }
    }
}

