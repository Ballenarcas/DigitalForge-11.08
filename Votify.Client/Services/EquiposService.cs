using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Votify.Client.Services
{
    public class EquiposService
    {
        private readonly HttpClient _httpClient;

        public EquiposService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EquipoDto>> ObtenerEquiposAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<EquipoDto>>("api/equipos");
            return response ?? new List<EquipoDto>();
        }

        public async Task<EquipoDto?> CrearEquipoAsync(string nombre)
        {
            var response = await _httpClient.PostAsJsonAsync("api/equipos", new { Nombre = nombre });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EquipoDto>();
            }
            return null;
        }

        public async Task<bool> AsignarParticipanteAsync(Guid equipoId, Guid participanteId, Guid eventoId)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/equipos/{equipoId}/miembros", new { ParticipanteId = participanteId, EventoId = eventoId });
            return response.IsSuccessStatusCode;
        }

        public async Task<EquipoDto?> ObtenerEquipoDeParticipanteAsync(Guid participanteId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<EquipoDto>($"api/equipos/participante/{participanteId}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }

    public class EquipoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
