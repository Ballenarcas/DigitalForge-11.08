namespace Votify.Client.DTOs
{
    public class VotarDto
    {
        public string VotacionId { get; set; } = string.Empty;
        public string ProyectoId { get; set; } = string.Empty;
        public string? VotanteId { get; set; }
    }
}
