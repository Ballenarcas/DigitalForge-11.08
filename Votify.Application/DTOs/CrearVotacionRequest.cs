namespace Votify.API.DTOs
{
    public class CrearVotacionRequest
    {
        public string Nombre { get; set; } = default!;
        public string Tipo { get; set; } = default!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int LimiteProyectos { get; set; }
        public bool PermiteComentarios { get; set; }
    }
}
