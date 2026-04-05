namespace Votify.Domain.Interfaces
{
    public interface IComentarioRepository
    {
        Task GuardarAsync(string proyectoId, string texto, Guid? autorId = null);
        Task<List<string>> ObtenerAsync(string proyectoId);
    }
}
