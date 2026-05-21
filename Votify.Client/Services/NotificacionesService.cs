using System.Net.Http.Json;
using Votify.Client.DTOs;

namespace Votify.Client.Services
{
    public class NotificacionesService
    {
        private readonly HttpClient _http;

        public NotificacionesService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<NotificacionDto>> ObtenerNotificacionesAsync()
        {
            return await _http.GetFromJsonAsync<List<NotificacionDto>>("api/notificaciones") ?? new List<NotificacionDto>();
        }

        public async Task<int> ObtenerNoLeidasCountAsync()
        {
            return await _http.GetFromJsonAsync<int>("api/notificaciones/no-leidas");
        }

        public async Task MarcarComoLeidaAsync(Guid id)
        {
            await _http.PutAsync($"api/notificaciones/{id}/leer", null);
        }
    }
}