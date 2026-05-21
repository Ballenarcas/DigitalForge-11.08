using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Votify.Client.Services
{
    public class NotificationService
    {
        private readonly HttpClient _httpClient;

        public NotificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NotificationClientDto>> ObtenerNotificacionesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<NotificationClientDto>>("api/notifications");
                return response ?? new List<NotificationClientDto>();
            }
            catch (Exception)
            {
                return new List<NotificationClientDto>();
            }
        }

        public async Task MarcarComoLeidasAsync()
        {
            try
            {
                await _httpClient.PostAsync("api/notifications/read", null);
            }
            catch (Exception)
            {
                // Ignorar fallos de red
            }
        }

        public async Task LimpiarNotificacionesAsync()
        {
            try
            {
                await _httpClient.PostAsync("api/notifications/clear", null);
            }
            catch (Exception)
            {
                // Ignorar fallos de red
            }
        }
    }

    public class NotificationClientDto
    {
        public Guid Id { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public Guid VotacionId { get; set; }
        public string VotacionNombre { get; set; } = string.Empty;
        public string TipoEvento { get; set; } = string.Empty;
        public bool Leido { get; set; }
    }
}
