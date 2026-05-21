namespace Votify.Client.DTOs
{
    public class ResultadoProyectoDto
    {
        public string Id { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string Equipo { get; set; } = default!;
        public int TotalVotos { get; set; }
        public double? PuntajeFinal { get; set; }
        public int Evaluaciones { get; set; }
        public int Posicion { get; set; }
        public bool IsManual { get; set; }
        public string? Justificacion { get; set; }
    }
}
