namespace Votify.Application.DTOs
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public required string Token { get; set; }
        public required string Message { get; set; }
    }
}