using System.Linq;

namespace Votify.Application.DTOs
{
    public class ParticipanteRolDto
    {
        public string? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Username => Email?.Split('@').FirstOrDefault() ?? Nombre;
    }
}
