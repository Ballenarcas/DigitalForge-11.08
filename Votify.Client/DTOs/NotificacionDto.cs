namespace Votify.Client.DTOs
{
    public class NotificacionDto
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? RecursoId { get; set; }
        public string? RecursoTipo { get; set; }
        public bool Leida { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}