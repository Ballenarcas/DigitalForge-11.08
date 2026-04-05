namespace Votify.Client.DTOs
{
    public class CrearComentarioRequest
    {
        public string Texto { get; set; } = string.Empty;
        public Guid? AutorId { get; set; }
    }
}
