namespace Votify.Client.DTOs
{
    public class VotacionDto
    {
        public string Id { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string Tipo { get; set; } = default!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int LimiteProy { get; set; }
        public bool Comentarios { get; set; }
        public bool ComentariosObligatorios { get; set; }
        public bool EsAnonima { get; set; }
        public string EventoId { get; set; } = default!;
        public int Estado { get; set; }
    }
}