namespace Votify.Application.DTOs
{
    public class EquipoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
