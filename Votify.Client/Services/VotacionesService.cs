using System.Net.Http.Json;
using Votify.Client.DTOs;
using Votify.Domain.Entities;

namespace Votify.Client.Services
{
    public class VotacionesService
    {
        private readonly HttpClient _http;

        public VotacionesService(HttpClient http)
        {
            _http = http;
        }

        public async Task CrearVotacion(VotacionDto request)
        {
            var response = await _http.PostAsJsonAsync("api/votaciones", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al crear la votación: {error}");
            }
        }

        public async Task<List<VotacionDto>> ObtenerVotaciones()
        {
            var resultado = await _http.GetFromJsonAsync<List<VotacionDto>>("api/votaciones");
            return resultado ?? new List<VotacionDto>();
        }
        public async Task<List<VotacionDto>> ObtenerVotacionesPorEvento(string eventoId)
        {
            var resultado = await _http.GetFromJsonAsync<List<VotacionDto>>($"api/votaciones/evento/{eventoId}");
            return resultado ?? new List<VotacionDto>();
        }

        public async Task<VotacionDto?> ObtenerVotacion(string id)
        {
            return await _http.GetFromJsonAsync<VotacionDto>($"api/votaciones/{id}");
        }

        public async Task ActualizarVotacion(string id, VotacionDto request)
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

        public async Task EmitirVotoMulticriterioAsync(VotoMulticriterioDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/votos/multicriterio", dto);

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
        public async Task<bool> VerificarLimiteVotos(string votacionId, string votanteId)
        {
            try
            {
                var response = await _http.GetAsync($"api/votos/puede-votar/{votacionId}/{votanteId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<bool>();
                }
                return true; // En caso de error, preferimos permitir el intento (el backend validará igual)
            }
            catch
            {
                return true;
            }
        }

        public async Task<bool> HaVotadoMulticriterio(string proyectoId, string votanteId)
        {
            try
            {
                var response = await _http.GetAsync($"api/votos/multicriterio/emitido/{proyectoId}/{votanteId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<bool>();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task PausarVotacion(string id)
        {
            var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
            var response = await _http.PatchAsync($"api/votaciones/{id}/pausar", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al pausar la votación: {error}");
            }
        }

        public async Task DetenerVotacion(string id)
        {
            var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
            var response = await _http.PatchAsync($"api/votaciones/{id}/detener", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al detener la votación: {error}");
            }
        }

        public async Task AbrirVotacion(string id)
        {
            var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
            var response = await _http.PatchAsync($"api/votaciones/{id}/abrir", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al abrir la votación: {error}");
            }
        }
    }
}
