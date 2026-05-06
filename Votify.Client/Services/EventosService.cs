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

        public async Task<List<ParticipanteRolDto>> ObtenerParticipantesPorEvento(string eventoId, string search = "")
        {
            var query = string.IsNullOrWhiteSpace(search) ? string.Empty : $"?search={Uri.EscapeDataString(search)}";
            var response = await _http.GetAsync($"api/eventos/{eventoId}/participantes{query}");
            await EnsureSuccessWithMessage(response);
            var resultado = await response.Content.ReadFromJsonAsync<List<ParticipanteRolDto>>();
            return resultado ?? new List<ParticipanteRolDto>();
        }

        public async Task<RoleStatisticsDto> ObtenerEstadisticasRoles(string eventoId)
        {
            var response = await _http.GetAsync($"api/eventos/{eventoId}/roles/count");
            await EnsureSuccessWithMessage(response);
            var resultado = await response.Content.ReadFromJsonAsync<RoleStatisticsDto>();
            return resultado ?? new RoleStatisticsDto();
        }

        public async Task CambiarRolParticipante(string eventoId, string participanteId, string rol)
        {
            var response = await _http.PutAsJsonAsync($"api/eventos/{eventoId}/participantes/{participanteId}/rol", new { Rol = rol });
            await EnsureSuccessWithMessage(response);
        }

        public async Task EliminarParticipanteDeEvento(string eventoId, string participanteId)
        {
            var response = await _http.DeleteAsync($"api/eventos/{eventoId}/participantes/{participanteId}");
            await EnsureSuccessWithMessage(response);
        }

        private static async Task EnsureSuccessWithMessage(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var error = await response.Content.ReadFromJsonAsync<ApiMessageResponse>();
            throw new InvalidOperationException(error?.Message ?? "No tienes permisos");
        }

        private class ApiMessageResponse { public string Message { get; set; } = ""; }
        private class UploadResponse { public string Url { get; set; } = ""; }
        private class RolResponse { public string Rol { get; set; } = ""; }
    }
}
