namespace Votify.Client.DTOs
{
    public class CriterioDto
    {
        public string? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Estrellas";
        public decimal Peso { get; set; }
    }
}
