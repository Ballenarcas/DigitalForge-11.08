using Votify.Client.DTOs;
using System.Net.Http.Json;

namespace Votify.Client.Services
{
    public class ProyectosService
    {
        private readonly HttpClient _httpClient;

        public ProyectosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProyectoDto>> ObtenerProyectosAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<ProyectoDto>>("api/proyectos");
            return response ?? new List<ProyectoDto>();
        }

        public async Task<ProyectoDto?> ObtenerProyectoAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<ProyectoDto>($"api/proyectos/{id}");
        }

        public async Task AgregarComentarioAsync(string proyectoId, string texto, string? autorId = null)
        {
            var request = new CrearComentarioRequest { Texto = texto, AutorId = string.IsNullOrEmpty(autorId) ? null : Guid.Parse(autorId) };
            var response = await _httpClient.PostAsJsonAsync($"api/proyectos/{proyectoId}/comentarios", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al enviar el comentario: {errorMsg}");
            }
        }

        public async Task<List<ComentarioDto>> ObtenerComentariosAsync(string proyectoId, string? votacionId = null)
        {
            var url = $"api/proyectos/{proyectoId}/comentarios";
            if (!string.IsNullOrEmpty(votacionId))
            {
                url += $"?votacionId={Uri.EscapeDataString(votacionId)}";
            }

            var response = await _httpClient.GetFromJsonAsync<List<ComentarioDto>>(url);
            return response ?? new List<ComentarioDto>();
        }
    }
}