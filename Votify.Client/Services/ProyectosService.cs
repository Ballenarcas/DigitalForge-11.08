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

        public async Task<List<ProyectoDto>> ObtenerProyectosAsync(string? votacionId = null)
        {
            var url = string.IsNullOrEmpty(votacionId) ? "api/proyectos" : $"api/proyectos/votacion/{votacionId}";
            var response = await _httpClient.GetFromJsonAsync<List<ProyectoDto>>(url);
            return response ?? new List<ProyectoDto>();
        }

        public async Task<ProyectoDto?> ObtenerProyectoAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<ProyectoDto>($"api/proyectos/{id}");
        }

        public async Task<string> CrearProyectoAsync(ProyectoDto proyecto)
        {
            var resp = await _httpClient.PostAsJsonAsync("api/proyectos", proyecto);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync();
        }

        public async Task<string> SubirImagen(MultipartFormDataContent content, string bucket = "proyectos")
        {
            var resp = await _httpClient.PostAsync($"api/files/upload?bucket={Uri.EscapeDataString(bucket)}", content);
            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadFromJsonAsync<UploadResponse>();
            
            if (result != null && !string.IsNullOrEmpty(result.Url))
            {
                return result.Url;
            }
            return "";
        }

        private class UploadResponse { public string Url { get; set; } = ""; }


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

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al obtener comentarios: {errorMsg}");
            }

            var comentarios = await response.Content.ReadFromJsonAsync<List<ComentarioDto>>();
            return comentarios ?? new List<ComentarioDto>();
        }

        public async Task<ResumenComentarioDto?> ObtenerResumenAsync(string proyectoId)
        {
            var response = await _httpClient.GetAsync($"api/proyectos/{proyectoId}/comentarios/resumen");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ResumenComentarioDto>();
        }
    }
}