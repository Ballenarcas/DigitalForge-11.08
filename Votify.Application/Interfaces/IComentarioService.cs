namespace Votify.Application.Interfaces
{
    public interface IComentarioService
    {
        Task AgregarComentarioAsync(string proyectoId, string texto, Guid? autorId = null);
        Task<List<string>> ObtenerComentariosAsync(string proyectoId);
    }
}
