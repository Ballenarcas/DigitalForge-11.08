namespace Votify.Application.DTOs
{
    public class ResultadoMulticriterioDto
    {
        public string Id { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string Equipo { get; set; } = default!;
        public double PuntajeFinal { get; set; }
        public int Evaluaciones { get; set; }
        public int Posicion { get; set; }
        public List<DetalleCriterioResultadoDto> DetallesCriterios { get; set; } = new();
    }

    public class DetalleCriterioResultadoDto
    {
        public string CriterioId { get; set; } = default!;
        public string CriterioNombre { get; set; } = default!;
        public decimal Peso { get; set; }
        public double PromedioValoracion { get; set; }
        public double PuntajePonderado { get; set; }
    }
}
