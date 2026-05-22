using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Votify.Domain.Interfaces;

namespace Votify.Infrastructure.Storage
{
    public class SupabaseStorageService : IStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;
        private readonly ILogger<SupabaseStorageService> _logger;

        public SupabaseStorageService(
            HttpClient httpClient,
            string supabaseUrl,
            string supabaseKey,
            ILogger<SupabaseStorageService> logger)
        {
            _httpClient = httpClient;
            _supabaseUrl = supabaseUrl.TrimEnd('/');
            _supabaseKey = supabaseKey;
            _logger = logger;
        }

        public async Task<string> SubirArchivoAsync(string bucket, Stream archivo, string nombreArchivo, string contentType)
        {
            using var ms = new MemoryStream();
            await archivo.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var url = $"{_supabaseUrl}/storage/v1/object/{bucket}/{nombreArchivo}";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("apikey", _supabaseKey);
            request.Headers.Add("Authorization", $"Bearer {_supabaseKey}");
            request.Content = new ByteArrayContent(bytes);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error subiendo archivo a Supabase Storage. Status={Status}, Body={Body}", (int)response.StatusCode, errorBody);
                throw new Exception($"Error al subir archivo a Supabase Storage: {(int)response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseContent);
            var path = doc.RootElement.GetProperty("Key").GetString();

            return $"{_supabaseUrl}/storage/v1/object/public/{bucket}/{nombreArchivo}";
        }

        public async Task EliminarArchivoAsync(string bucket, string rutaArchivo)
        {
            var nombreArchivo = Path.GetFileName(new Uri(rutaArchivo).LocalPath);

            var url = $"{_supabaseUrl}/storage/v1/object/{bucket}/{nombreArchivo}";

            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Add("apikey", _supabaseKey);
            request.Headers.Add("Authorization", $"Bearer {_supabaseKey}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error eliminando archivo de Supabase Storage. Status={Status}, Body={Body}", (int)response.StatusCode, errorBody);
            }
        }
    }
}