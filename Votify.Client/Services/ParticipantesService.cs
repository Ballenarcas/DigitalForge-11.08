using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Votify.Client.Services
{
    public class ParticipantesService
    {
        private readonly HttpClient _httpClient;

        public ParticipantesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ParticipanteDto>> ObtenerParticipantesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<ParticipanteDto>>("api/participantes");
            return response ?? new List<ParticipanteDto>();
        }
        public async Task<ParticipanteDto?> ObtenerParticipantePorId(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ParticipanteDto>($"api/participantes/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }

    public class ParticipanteDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid? EquipoId { get; set; }
    }
}
