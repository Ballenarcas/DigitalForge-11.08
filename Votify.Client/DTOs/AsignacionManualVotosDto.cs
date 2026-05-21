namespace Votify.Client.DTOs
{
    public class AsignacionManualVotosDto
    {
        public string ProyectoId { get; set; } = default!;
        public int PosicionFinal { get; set; }
        public int VotosAsignados { get; set; }
        public string? Justificacion { get; set; }
    }
}
