using System.Net.Http.Json;
using Votify.Client.DTOs;

namespace Votify.Client.Services
{
    public class EventosService
    {
        private readonly HttpClient _http;

        public EventosService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<EventoDto>> ObtenerEventosGenerales()
        {
            var resultado = await _http.GetFromJsonAsync<List<EventoDto>>("api/eventos");
            return resultado ?? new List<EventoDto>();
        }

        public async Task<List<EventoDto>> ObtenerMisEventos()
        {
            var resultado = await _http.GetFromJsonAsync<List<EventoDto>>("api/eventos/mis-eventos");
            return resultado ?? new List<EventoDto>();
        }

        public async Task<EventoDto?> ObtenerEvento(string id)
        {
            return await _http.GetFromJsonAsync<EventoDto>($"api/eventos/{id}");
        }

        public async Task<EventoDto> CrearEvento(EventoDto evento)
        {
            var resp = await _http.PostAsJsonAsync("api/eventos", evento);
            resp.EnsureSuccessStatusCode();
            return (await resp.Content.ReadFromJsonAsync<EventoDto>())!;
        }

        public async Task ParticiparEnEvento(string eventoId)
        {
            var resp = await _http.PostAsync($"api/eventos/{eventoId}/participar", null);
            resp.EnsureSuccessStatusCode();
        }

        public async Task<string> SubirImagen(MultipartFormDataContent content)
        {
            var resp = await _http.PostAsync("api/files/upload", content);
            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadFromJsonAsync<UploadResponse>();
            
            if (result != null && !string.IsNullOrEmpty(result.Url))
            {
                // Return absolute URL for uploaded images
                return _http.BaseAddress + result.Url;
            }
            return "";
        }

        public async Task<string?> ObtenerRolEnEvento(string eventoId)
        {
            try
            {
                var resp = await _http.GetFromJsonAsync<RolResponse>($"api/eventos/{eventoId}/rol");
                return resp?.Rol;
            }
            catch
            {
                return null;
            }
        }

        private class UploadResponse { public string Url { get; set; } = ""; }
        private class RolResponse { public string Rol { get; set; } = ""; }
    }
}
