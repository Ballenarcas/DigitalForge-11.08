namespace Votify.Client.DTOs
{
    public class ResumenComentarioDto
    {
        public string Resumen { get; set; } = string.Empty;
        public int TotalComentarios { get; set; }
        public bool GeneradoPorIA { get; set; }
        public DateTime FechaGeneracion { get; set; }
    }

    public class ComentarioResumenItemDto
    {
        public string Texto { get; set; } = string.Empty;
        public string? AutorNombre { get; set; }
        public bool EsAnonimo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}