using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IManualVotosService
    {
        Task GuardarAsignacionManualAsync(string votacionId, string participanteId, AsignacionManualVotosDto dto);
        Task<List<ResultadoProyectoDto>> ObtenerAsignacionesManualesAsync(string votacionId);
        Task EliminarAsignacionManualAsync(string votacionId, string proyectoId);
        Task GuardarJustificacionAsync(string votacionId, string participanteId, GuardarJustificacionDto dto);
        Task<JustificacionDto?> ObtenerJustificacionAsync(string votacionId, string proyectoId);
    }
}
