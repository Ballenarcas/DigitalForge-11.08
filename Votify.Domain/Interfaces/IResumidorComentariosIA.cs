namespace Votify.Domain.Interfaces;

public interface IResumidorComentariosIA
{
    Task<ResumenComentario> ResumirComentariosAsync(
        List<ComentarioResumenItem> comentarios,
        string proyectoNombre);

    Task<bool> EstaDisponibleAsync();
}

public class ComentarioResumenItem
{
    public string Texto { get; set; } = string.Empty;
    public string? AutorNombre { get; set; }
    public bool EsAnonimo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class ResumenComentario
{
    public string Resumen { get; set; } = string.Empty;
    public int TotalComentarios { get; set; }
    public bool GeneradoPorIA { get; set; }
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
}