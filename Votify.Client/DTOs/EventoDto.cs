namespace Votify.Client.DTOs
{
    public class EventoDto
    {
        public string Id { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
