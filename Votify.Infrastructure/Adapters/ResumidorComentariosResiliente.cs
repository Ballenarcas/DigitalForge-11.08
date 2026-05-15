using Microsoft.Extensions.Logging;
using Votify.Domain.Interfaces;

namespace Votify.Infrastructure.Adapters;

public class ResumidorComentariosResiliente : IResumidorComentariosIA
{
    private readonly AdaptadorClienteIA _primary;
    private readonly ResumidorComentariosFallback _fallback;
    private readonly ILogger<ResumidorComentariosResiliente> _logger;

    public ResumidorComentariosResiliente(
        AdaptadorClienteIA primary,
        ResumidorComentariosFallback fallback,
        ILogger<ResumidorComentariosResiliente> logger)
    {
        _primary = primary;
        _fallback = fallback;
        _logger = logger;
    }

    public async Task<ResumenComentario> ResumirComentariosAsync(
        List<ComentarioResumenItem> comentarios,
        string proyectoNombre)
    {
        try
        {
            return await _primary.ResumirComentariosAsync(comentarios, proyectoNombre);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Adapter IA fallo para '{Proyecto}'. Usando fallback.", proyectoNombre);
            return await _fallback.ResumirComentariosAsync(comentarios, proyectoNombre);
        }
    }

    public async Task<bool> EstaDisponibleAsync()
    {
        try { return await _primary.EstaDisponibleAsync(); }
        catch { return false; }
    }
}