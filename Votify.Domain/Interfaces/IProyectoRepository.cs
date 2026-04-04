namespace Votify.Domain.Interfaces
{
    public interface IProyectoRepository
    {
        Task GuardarAsync(Proyecto proyecto);

        Task<List<Proyecto>> ObtenerTodasAsync();
        Task<Proyecto?> ObtenerAsync(String proyectoId);
        Task<List<Proyecto>> ObtenerPorVotacionAsync(string votacionId);
    }
}