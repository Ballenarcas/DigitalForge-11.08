namespace Votify.Client.DTOs
{
    public class ComentarioDto
    {
        public string Texto { get; set; } = string.Empty;
        public Guid? AutorId { get; set; }
        public bool EsAnonimo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
