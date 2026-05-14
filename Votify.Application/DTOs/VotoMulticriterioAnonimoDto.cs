namespace Votify.Application.DTOs
{
    public class VotoMulticriterioAnonimoDto
    {
        public string VotacionId { get; set; } = string.Empty;
        public string ProyectoId { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
        public List<ValoracionCriterioDto> Valoraciones { get; set; } = new();
    }
}