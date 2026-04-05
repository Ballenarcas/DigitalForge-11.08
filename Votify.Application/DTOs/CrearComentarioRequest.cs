namespace Votify.API.DTOs
{
    public class CrearComentarioRequest
    {
        public string Texto { get; set; } = string.Empty;
        public Guid? AutorId { get; set; }
    }
}
