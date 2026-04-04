using System.Net.Http.Json;
using Votify.Client.DTOs;

namespace Votify.Client.Services
{
    public class VotacionesService
    {
        private readonly HttpClient _http;

        public VotacionesService(HttpClient http)
        {
            _http = http;
        }

        public async Task CrearVotacion(CrearVotacionRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/votaciones", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al crear la votación: {error}");
            }
        }

        public async Task<List<CrearVotacionResponse>> ObtenerVotaciones()
        {
            var resultado = await _http.GetFromJsonAsync<List<CrearVotacionResponse>>("api/votaciones");
            return resultado ?? new List<CrearVotacionResponse>();
        }

        public async Task<CrearVotacionResponse?> ObtenerVotacion(string id)
        {
            return await _http.GetFromJsonAsync<CrearVotacionResponse>($"api/votaciones/{id}");
        }

        public async Task ActualizarVotacion(string id, CrearVotacionRequest request)
        {
            var response = await _http.PutAsJsonAsync($"api/votaciones/{id}", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al actualizar la votación: {error}");
            }
        }

        public async Task EliminarVotacion(string id)
        {
            var response = await _http.DeleteAsync($"api/votaciones/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al eliminar la votación: {error}");
            }
        }

        public async Task EmitirVotoAsync(string votacionId, VotarDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/votos", dto);

            if (!response.IsSuccessStatusCode)
            {

                var error = await response.Content.ReadAsStringAsync();
                

                if (error.Contains("Error"))
                {
                    throw new Exception(error);
                }
                throw new Exception($"Error HTTP: {response.StatusCode} - {error}");
            }
        }

        public async Task<List<ResultadoProyectoDto>?> ObtenerResultados(string votacionId)
        {
            return await _http.GetFromJsonAsync<List<ResultadoProyectoDto>>($"api/votaciones/{votacionId}/resultados");
        }
    }
}