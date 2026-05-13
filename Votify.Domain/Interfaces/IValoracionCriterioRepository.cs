using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IValoracionCriterioRepository
    {
        Task<bool> HaValoradoProyectoAsync(string proyectoId, string votanteId);
        Task GuardarAsync(string proyectoId, string votanteId, List<ValoracionCriterio> valoraciones);
        Task<List<ValoracionCriterio>> ObtenerPorProyectoYVotanteAsync(string proyectoId, string votanteId);
        Task<List<(string ProyectoId, double Puntaje, int Evaluaciones)>> ObtenerResultadosPonderadosAsync(string votacionId);
        Task<List<(string ProyectoId, Guid CriterioId, double PromedioValoracion, int NumEvaluaciones)>> ObtenerDetallesPorCriterioAsync(string votacionId);
        Task EliminarPorVotacionAsync(string votacionId);
    }
}
