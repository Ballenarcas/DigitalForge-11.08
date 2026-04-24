namespace Votify.Application.DTOs
{
    public class RegisterRequestDto
    {
        public required string Nombre { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}