using System.Threading.Tasks;
using Votify.Application.DTOs; // Utiliza el namespace establecido en tus DTOs del cliente

namespace Votify.Client.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task LogoutAsync();
    }
}