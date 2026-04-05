namespace Votify.Application.Interfaces
{
    public interface IComentarioService
    {
        Task AgregarComentarioAsync(string proyectoId, string texto);
        Task<List<string>> ObtenerComentariosAsync(string proyectoId);
    }
}
