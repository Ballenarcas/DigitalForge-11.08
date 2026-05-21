using System;

namespace Votify.Application.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public Guid VotacionId { get; set; }
        public string VotacionNombre { get; set; } = string.Empty;
        public string TipoEvento { get; set; } = string.Empty;
        public bool Leido { get; set; }
    }
}
