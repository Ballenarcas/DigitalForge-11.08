using System.Text;
using Microsoft.Extensions.Logging;
using Votify.Domain.Interfaces;

namespace Votify.Infrastructure.Adapters;

public class ResumidorComentariosFallback : IResumidorComentariosIA
{
    private readonly ILogger<ResumidorComentariosFallback> _logger;

    public ResumidorComentariosFallback(ILogger<ResumidorComentariosFallback> logger)
    {
        _logger = logger;
    }

    public Task<ResumenComentario> ResumirComentariosAsync(
        List<ComentarioResumenItem> comentarios,
        string proyectoNombre)
    {
        _logger.LogDebug("Usando fallback para '{Proyecto}' con {Count} comentarios.",
            proyectoNombre, comentarios.Count);

        if (comentarios.Count == 0)
        {
            return Task.FromResult(new ResumenComentario
            {
                Resumen = "No hay comentarios para resumir.",
                TotalComentarios = 0,
                GeneradoPorIA = false,
                FechaGeneracion = DateTime.UtcNow
            });
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Resumen de {comentarios.Count} comentario(s) para \"{proyectoNombre}\":");
        sb.AppendLine();

        foreach (var c in comentarios)
        {
            var autor = c.EsAnonimo ? "Anonimo" : (c.AutorNombre ?? "Participante");
            sb.AppendLine($"- {autor} ({c.FechaCreacion:dd/MM/yyyy}): {c.Texto}");
        }

        return Task.FromResult(new ResumenComentario
        {
            Resumen = sb.ToString(),
            TotalComentarios = comentarios.Count,
            GeneradoPorIA = false,
            FechaGeneracion = DateTime.UtcNow
        });
    }

    public Task<bool> EstaDisponibleAsync() => Task.FromResult(true);
}