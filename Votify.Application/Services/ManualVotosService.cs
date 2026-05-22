using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using EstadoFinalizada = Votify.Domain.Estado.EstadoFinalizada;

namespace Votify.Application.Services
{
    public class ManualVotosService : IManualVotosService
    {
        private readonly IManualVotosAsignacionRepository _repo;
        private readonly IProyectoRepository _proyectoRepo;
        private readonly IVotacionRepository _votacionRepo;
        private readonly IEquipoRepository _equipoRepo;

        public ManualVotosService(
            IManualVotosAsignacionRepository repo,
            IProyectoRepository proyectoRepo,
            IVotacionRepository votacionRepo,
            IEquipoRepository equipoRepo)
        {
            _repo = repo;
            _proyectoRepo = proyectoRepo;
            _votacionRepo = votacionRepo;
            _equipoRepo = equipoRepo;
        }

        public async Task GuardarAsignacionManualAsync(string votacionId, string participanteId, AsignacionManualVotosDto dto)
        {
            if (string.IsNullOrEmpty(votacionId) || string.IsNullOrEmpty(participanteId))
                throw new ArgumentException("VotacionId y ParticipanteId son requeridos");

            if (dto.PosicionFinal <= 0 || dto.VotosAsignados < 0)
                throw new ArgumentException("Posición y votos deben ser válidos");

            var votacion = await _votacionRepo.ObtenerAsync(votacionId);
            if (votacion == null)
                throw new KeyNotFoundException("Votación no encontrada");

            if (votacion.Estado is EstadoFinalizada)
                throw new InvalidOperationException("No se pueden asignar votos manuales a votaciones finalizadas");

            var existente = await _repo.ObtenerPorVotacionAsync(Guid.Parse(votacionId));
            var asignacionExistente = existente.FirstOrDefault(a => a.ProyectoId == Guid.Parse(dto.ProyectoId));

            if (asignacionExistente != null)
            {
                asignacionExistente.PosicionFinal = dto.PosicionFinal;
                asignacionExistente.VotosAsignados = dto.VotosAsignados;
                asignacionExistente.FechaCreacion = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(dto.Justificacion))
                {
                    asignacionExistente.TextoJustificacion = dto.Justificacion;
                    asignacionExistente.UsuarioJustificacion = participanteId;
                    asignacionExistente.RolUsuarioJustificacion = "ORGANIZADOR";
                    asignacionExistente.FechaJustificacion = DateTime.UtcNow;
                }
                await _repo.GuardarAsync(asignacionExistente);
            }
            else
            {
                var asignacion = new ManualVotosAsignacion
                {
                    VotacionId = Guid.Parse(votacionId),
                    ProyectoId = Guid.Parse(dto.ProyectoId),
                    PosicionFinal = dto.PosicionFinal,
                    VotosAsignados = dto.VotosAsignados,
                    CreadoPor = participanteId,
                    FechaCreacion = DateTime.UtcNow,
                    TextoJustificacion = dto.Justificacion,
                    UsuarioJustificacion = !string.IsNullOrEmpty(dto.Justificacion) ? participanteId : null,
                    RolUsuarioJustificacion = !string.IsNullOrEmpty(dto.Justificacion) ? "ORGANIZADOR" : null,
                    FechaJustificacion = !string.IsNullOrEmpty(dto.Justificacion) ? DateTime.UtcNow : null
                };
                await _repo.GuardarAsync(asignacion);
            }
        }

        public async Task<List<ResultadoProyectoDto>> ObtenerAsignacionesManualesAsync(string votacionId)
        {
            if (string.IsNullOrEmpty(votacionId))
                throw new ArgumentException("VotacionId es requerido");

            var asignaciones = await _repo.ObtenerPorVotacionAsync(Guid.Parse(votacionId));

            var resultados = new List<ResultadoProyectoDto>();

            foreach (var asignacion in asignaciones)
            {
                var proyecto = await _proyectoRepo.ObtenerAsync(asignacion.ProyectoId.ToString());
                var equipo = proyecto?.Equipo_Id != null ? await _equipoRepo.ObtenerPorIdAsync(Guid.Parse(proyecto.Equipo_Id)) : null;

                resultados.Add(new ResultadoProyectoDto
                {
                    Id = proyecto?.Id ?? string.Empty,
                    Nombre = proyecto?.Nombre ?? "Desconocido",
                    Equipo = equipo?.Nombre ?? "Sin Equipo",
                    TotalVotos = asignacion.VotosAsignados,
                    Posicion = asignacion.PosicionFinal,
                    IsManual = true
                });
            }

            return resultados.OrderBy(r => r.Posicion).ToList();
        }

        public async Task GuardarJustificacionAsync(string votacionId, string participanteId, GuardarJustificacionDto dto)
        {
            if (string.IsNullOrEmpty(votacionId))
                throw new ArgumentException("VotacionId es requerido");
            if (string.IsNullOrEmpty(dto.ProyectoId))
                throw new ArgumentException("ProyectoId es requerido");
            if (string.IsNullOrEmpty(dto.TextoJustificacion?.Trim()))
                throw new ArgumentException("La justificación no puede estar vacía");

            var asignaciones = await _repo.ObtenerPorVotacionAsync(Guid.Parse(votacionId));
            var asignacion = asignaciones.FirstOrDefault(a => a.ProyectoId == Guid.Parse(dto.ProyectoId));

            if (asignacion == null)
                throw new ArgumentException("No existe asignación para este proyecto en esta votación");

            asignacion.TextoJustificacion = dto.TextoJustificacion;
            asignacion.UsuarioJustificacion = participanteId;
            asignacion.RolUsuarioJustificacion = "ORGANIZADOR";
            asignacion.FechaJustificacion = DateTime.UtcNow;

            await _repo.GuardarAsync(asignacion);
        }

        public async Task<JustificacionDto?> ObtenerJustificacionAsync(string votacionId, string proyectoId)
        {
            if (string.IsNullOrEmpty(votacionId) || string.IsNullOrEmpty(proyectoId))
                return null;

            var asignaciones = await _repo.ObtenerPorVotacionAsync(Guid.Parse(votacionId));
            var asignacion = asignaciones.FirstOrDefault(a => a.ProyectoId == Guid.Parse(proyectoId));

            if (asignacion?.TextoJustificacion == null)
                return null;

            return new JustificacionDto
            {
                ProyectoId = proyectoId,
                TextoJustificacion = asignacion.TextoJustificacion,
                UsuarioNombre = asignacion.UsuarioJustificacion ?? "Desconocido",
                RolUsuario = asignacion.RolUsuarioJustificacion ?? "USUARIO",
                FechaJustificacion = asignacion.FechaJustificacion ?? DateTime.UtcNow
            };
        }

        public async Task EliminarAsignacionManualAsync(string votacionId, string proyectoId)
        {
            if (string.IsNullOrEmpty(votacionId) || string.IsNullOrEmpty(proyectoId))
                throw new ArgumentException("VotacionId y ProyectoId son requeridos");

            await _repo.EliminarAsync(Guid.Parse(votacionId), Guid.Parse(proyectoId));
        }
    }
}
