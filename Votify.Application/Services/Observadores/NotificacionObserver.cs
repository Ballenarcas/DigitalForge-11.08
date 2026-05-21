using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services.Observadores
{
    /// <summary>
    /// Observador que gestiona el envío de notificaciones a organizadores de eventos.
    /// Implementa el Patrón Observer para reaccionar ante transiciones de estado de votaciones,
    /// creación de equipos y proyectos.
    /// </summary>
    public class NotificacionObserver : IVotacionObserver, INotificacionObserver
    {
        private readonly INotificacionService _notificacionService;
        private readonly IParticipanteEventoRepository _participanteEventoRepo;
        private readonly IVotacionRepository _votacionRepo;
        private readonly ILogger<NotificacionObserver> _logger;

        public NotificacionObserver(
            INotificacionService notificacionService,
            IParticipanteEventoRepository participanteEventoRepo,
            IVotacionRepository votacionRepo,
            ILogger<NotificacionObserver> logger)
        {
            _notificacionService = notificacionService;
            _participanteEventoRepo = participanteEventoRepo;
            _votacionRepo = votacionRepo;
            _logger = logger;
        }

        public async Task OnVotacionCreadaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación creada: {VotacionId} - Nombre: {Nombre} - Tipo: {Tipo}",
                votacion.Id, votacion.Nombre, votacion.Tipo);

            var organizadores = await _participanteEventoRepo.ObtenerOrganizadoresIdsAsync(votacion.EventoId);
            foreach (var orgId in organizadores)
            {
                await _notificacionService.CrearNotificacionAsync(
                    orgId,
                    $"Se ha creado la votación '{votacion.Nombre}' en tu evento.",
                    "VOTACION_CREADA",
                    votacion.Id.ToString(),
                    "VOTACION"
                );
            }
        }

        public async Task OnVotacionPausadaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación pausada: {VotacionId} - Nombre: {Nombre}",
                votacion.Id, votacion.Nombre);

            var organizadores = await _participanteEventoRepo.ObtenerOrganizadoresIdsAsync(votacion.EventoId);
            foreach (var orgId in organizadores)
            {
                await _notificacionService.CrearNotificacionAsync(
                    orgId,
                    $"La votación '{votacion.Nombre}' ha sido pausada.",
                    "VOTACION_PAUSADA",
                    votacion.Id.ToString(),
                    "VOTACION"
                );
            }
        }

        public async Task OnVotacionDetenidaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación detenida: {VotacionId} - Nombre: {Nombre}",
                votacion.Id, votacion.Nombre);

            var organizadores = await _participanteEventoRepo.ObtenerOrganizadoresIdsAsync(votacion.EventoId);
            foreach (var orgId in organizadores)
            {
                await _notificacionService.CrearNotificacionAsync(
                    orgId,
                    $"La votación '{votacion.Nombre}' ha sido detenida.",
                    "VOTACION_DETENIDA",
                    votacion.Id.ToString(),
                    "VOTACION"
                );
            }
        }

        public async Task OnVotacionAbiertaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación reabierta/reanudada: {VotacionId} - Nombre: {Nombre}",
                votacion.Id, votacion.Nombre);

            var organizadores = await _participanteEventoRepo.ObtenerOrganizadoresIdsAsync(votacion.EventoId);
            foreach (var orgId in organizadores)
            {
                await _notificacionService.CrearNotificacionAsync(
                    orgId,
                    $"La votación '{votacion.Nombre}' ha sido iniciada/reanudada.",
                    "VOTACION_ABIERTA",
                    votacion.Id.ToString(),
                    "VOTACION"
                );
            }
        }

        public async Task OnEquipoCreadoAsync(Equipo equipo, string nombreEvento)
        {
            _logger.LogInformation("Equipo creado: {EquipoId} - Nombre: {Nombre} - Evento: {Evento}",
                equipo.Id, equipo.Nombre, nombreEvento);
        }

        public async Task OnProyectoCreadoAsync(Proyecto proyecto, string nombreVotacion)
        {
            _logger.LogInformation("Proyecto creado: {ProyectoId} - Nombre: {Nombre} - Votacion: {Votacion}",
                proyecto.Id, proyecto.Nombre, nombreVotacion);

            var votacionEntity = await _votacionRepo.ObtenerAsync(proyecto.VotacionId.ToString());
            if (votacionEntity == null) return;

            var organizadores = await _participanteEventoRepo.ObtenerOrganizadoresIdsAsync(votacionEntity.EventoId);
            foreach (var orgId in organizadores)
            {
                await _notificacionService.CrearNotificacionAsync(
                    orgId,
                    $"Se ha creado el proyecto '{proyecto.Nombre}' en la votación '{nombreVotacion}'.",
                    "PROYECTO_CREADO",
                    proyecto.Id.ToString(),
                    "PROYECTO"
                );
            }
        }
    }
}
