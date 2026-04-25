using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Votify.Application.DTOs;

namespace Votify.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/registro", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>() ?? new AuthResponseDto { IsSuccess = false, Message = "Respuesta vacía", Token = "" };
            }

            return new AuthResponseDto { IsSuccess = false, Message = "Error al intentar registrarse. Revisa los datos.", Token = "" };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                
                if (result != null && result.IsSuccess)
                {
                    // Guardar el Token en LocalStorage
                    await _localStorage.SetItemAsync("authToken", result.Token);
                    
                    // Notificar al AuthenticationStateProvider que iniciamos sesión
                    ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
                }
                
                return result ?? new AuthResponseDto { IsSuccess = false, Message = "Respuesta vacía", Token = "" };
            }

            return new AuthResponseDto { IsSuccess = false, Message = "Credenciales incorrectas.", Token = "" };
        }

        public async Task LogoutAsync()
        {
            // Remover el Token del LocalStorage
            await _localStorage.RemoveItemAsync("authToken");
            
            // Notificar al AuthenticationStateProvider que cerramos sesión
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }
    }
}