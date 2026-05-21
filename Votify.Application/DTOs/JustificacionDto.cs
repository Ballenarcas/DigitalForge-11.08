namespace Votify.Application.DTOs
{
    public class JustificacionDto
    {
        public string ProyectoId { get; set; } = default!;
        public string TextoJustificacion { get; set; } = default!;
        public string UsuarioNombre { get; set; } = default!;
        public string RolUsuario { get; set; } = default!;
        public DateTime FechaJustificacion { get; set; }
    }
}
