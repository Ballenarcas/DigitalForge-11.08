using Microsoft.Extensions.Logging;
using Votify.Domain.Interfaces;

namespace Votify.Infrastructure.Adapters;

public class ResumidorComentariosResiliente : IResumidorComentariosIA
{
    private readonly IResumidorComentariosIA _primary;
    private readonly IResumidorComentariosIA _fallback;
    private readonly ILogger<ResumidorComentariosResiliente> _logger;

    public ResumidorComentariosResiliente(
        IResumidorComentariosIA primary,
        IResumidorComentariosIA fallback,
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