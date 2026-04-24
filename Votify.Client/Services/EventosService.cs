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

        public async Task<List<EventoDto>> ObtenerEventos()
        {
            var resultado = await _http.GetFromJsonAsync<List<EventoDto>>("api/eventos");
            return resultado ?? new List<EventoDto>();
        }

        public async Task<EventoDto?> ObtenerEvento(string id)
        {
            return await _http.GetFromJsonAsync<EventoDto>($"api/eventos/{id}");
        }
    }
}
