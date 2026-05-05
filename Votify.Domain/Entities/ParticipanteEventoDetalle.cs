namespace Votify.Domain.Entities
{
    public class ParticipanteEventoDetalle
    {
        public Guid ParticipanteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
