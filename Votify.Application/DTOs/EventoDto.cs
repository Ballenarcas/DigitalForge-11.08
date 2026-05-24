using System.Collections.Generic;

namespace Votify.Application.DTOs
{
    public class EventoDto
    {
        public string? Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? ImagenUrl { get; set; }
        public List<string> Categorias { get; set; } = new();
    }
}
