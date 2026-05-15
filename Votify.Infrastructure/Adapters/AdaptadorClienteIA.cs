using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Votify.Domain.Interfaces;
using Votify.Infrastructure.Configuration;

namespace Votify.Infrastructure.Adapters;

public class AdaptadorClienteIA : IResumidorComentariosIA
{
    private readonly HttpClient _httpClient;
    private readonly OpcionesResumidorIA _options;
    private readonly ILogger<AdaptadorClienteIA> _logger;

    public AdaptadorClienteIA(
        HttpClient httpClient,
        IOptions<OpcionesResumidorIA> options,
        ILogger<AdaptadorClienteIA> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<ResumenComentario> ResumirComentariosAsync(
        List<ComentarioResumenItem> comentarios,
        string proyectoNombre)
    {
        if (!_options.Enabled || comentarios.Count == 0)
        {
            return CrearResultadoFallback(comentarios, proyectoNombre);
        }

        try
        {
            var prompt = ConstruirPrompt(comentarios, proyectoNombre);
            var requestBody = ConstruirCuerpoRequest(prompt);
            var response = await EnviarRequestAsync(requestBody);
            var summaryText = ParsearRespuesta(response);

            return new ResumenComentario
            {
                Resumen = summaryText,
                TotalComentarios = comentarios.Count,
                GeneradoPorIA = true,
                FechaGeneracion = DateTime.UtcNow
            };
        }
        catch (HttpRequestException hex) when (hex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogError("Modelo IA no encontrado (404). Modelo='{Model}'. Cambialo a gemini-2.0-flash en .env", _options.Model);
            return CrearResultadoFallback(comentarios, proyectoNombre);
        }
        catch (HttpRequestException hex) when (hex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            _logger.LogError("Rate limit de Google AI Studio alcanzado (429). Espera 1 minuto o usa otra API key. Limite gratis: 15 req/min, 1000 req/dia.");
            return CrearResultadoFallback(comentarios, proyectoNombre);
        }
        catch (HttpRequestException hex) when (hex.StatusCode == System.Net.HttpStatusCode.Unauthorized || hex.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            _logger.LogError("API key de IA invalida ({Status}). Revisa AI_SUMMARIZER_API_KEY en .env", (int)hex.StatusCode);
            return CrearResultadoFallback(comentarios, proyectoNombre);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando resumen IA para '{Proyecto}'.", proyectoNombre);
            return CrearResultadoFallback(comentarios, proyectoNombre);
        }
    }

    public async Task<bool> EstaDisponibleAsync()
    {
        if (!_options.Enabled)
        {
            _logger.LogWarning("Resumidor IA deshabilitado. Setea AI_SUMMARIZER_ENABLED=true en .env");
            return false;
        }
        if (string.IsNullOrEmpty(_options.ApiKey))
        {
            _logger.LogWarning("API key de IA vacia. Setea AI_SUMMARIZER_API_KEY en .env");
            return false;
        }
        try
        {
            var url = $"{_options.BaseUrl}models/{_options.Model}?key={_options.ApiKey}";
            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError("Modelo IA no encontrado (404). Modelo='{Model}'. Usa un modelo valido como gemini-2.0-flash", _options.Model);
                return false;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogError("API key de IA invalida ({Status}). Revisa AI_SUMMARIZER_API_KEY en .env", (int)response.StatusCode);
                return false;
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verificando disponibilidad de IA");
            return false;
        }
    }

    private string ConstruirPrompt(List<ComentarioResumenItem> comentarios, string proyectoNombre)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Eres un asistente que consolida comentarios de feedback en un resumen unificado.");
        sb.AppendLine($"Proyecto: {proyectoNombre}");
        sb.AppendLine($"Genera un resumen en espanol que:");
        sb.AppendLine("1. Identifique los temas principales mencionados.");
        sb.AppendLine("2. Destaque fortalezas y areas de mejora.");
        sb.AppendLine("3. Mantenga un tono constructivo y profesional.");
        sb.AppendLine("4. Sea conciso (maximo 3 parrafos).");
        sb.AppendLine();
        sb.AppendLine("Comentarios:");
        for (int i = 0; i < comentarios.Count; i++)
        {
            var c = comentarios[i];
            var autor = c.EsAnonimo ? "Anonimo" : (c.AutorNombre ?? $"Comentador {i + 1}");
            sb.AppendLine($"- [{autor}, {c.FechaCreacion:dd/MM/yyyy}]: {c.Texto}");
        }
        return sb.ToString();
    }

    private object ConstruirCuerpoRequest(string prompt)
    {
        return new
        {
            systemInstruction = new
            {
                parts = new[]
                {
                    new { text = "Eres un asistente que genera resumenes constructivos de feedback." }
                }
            },
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.3,
                maxOutputTokens = _options.MaxTokens
            }
        };
    }

    private async Task<string> EnviarRequestAsync(object requestBody)
    {
        var url = $"{_options.BaseUrl}models/{_options.Model}:generateContent?key={_options.ApiKey}";
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private string ParsearRespuesta(string responseBody)
    {
        using var doc = JsonDocument.Parse(responseBody);
        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "No se pudo generar el resumen.";
    }

    private ResumenComentario CrearResultadoFallback(
        List<ComentarioResumenItem> comentarios, string proyectoNombre)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Resumen de {comentarios.Count} comentario(s) para \"{proyectoNombre}\":");
        foreach (var c in comentarios)
        {
            var autor = c.EsAnonimo ? "Anonimo" : (c.AutorNombre ?? "Participante");
            sb.AppendLine($"- {autor}: {c.Texto}");
        }
        return new ResumenComentario
        {
            Resumen = sb.ToString(),
            TotalComentarios = comentarios.Count,
            GeneradoPorIA = false,
            FechaGeneracion = DateTime.UtcNow
        };
    }
}