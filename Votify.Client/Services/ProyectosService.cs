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

        public async Task AgregarComentarioAsync(string proyectoId, string texto)
        {
            var request = new CrearComentarioRequest { Texto = texto };
            var response = await _httpClient.PostAsJsonAsync($"api/proyectos/{proyectoId}/comentarios", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al enviar el comentario: {errorMsg}");
            }
        }

        public async Task<List<string>> ObtenerComentariosAsync(string proyectoId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<string>>($"api/proyectos/{proyectoId}/comentarios");
            return response ?? new List<string>();
        }
    }
}