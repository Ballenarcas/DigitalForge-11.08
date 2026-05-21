using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services.Observadores
{
    /// <summary>
    /// Observador que registra eventos de notificación en el ciclo de vida de las votaciones.
    /// Este observador puede ser extendido para enviar notificaciones a usuarios, registrar auditoría, etc.
    /// </summary>
    public class NotificacionObserver : IVotacionObserver
    {
        private readonly ILogger<NotificacionObserver> _logger;
        private readonly INotificacionService _notificacionService;

        public NotificacionObserver(ILogger<NotificacionObserver> logger, INotificacionService notificacionService)
        {
            _logger = logger;
            _notificacionService = notificacionService;
        }

        public Task OnVotacionCreadaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación creada: {VotacionId} - Nombre: {Nombre} - Tipo: {Tipo}",
                votacion.Id, votacion.Nombre, votacion.Tipo);

            _notificacionService.AgregarNotificacion(
                $"Nueva votación '{votacion.Nombre}' del tipo {votacion.Tipo} ha sido creada.",
                "Creada",
                votacion
            );

            return Task.CompletedTask;
        }

        public Task OnVotacionPausadaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación pausada: {VotacionId} - Nombre: {Nombre}",
                votacion.Id, votacion.Nombre);

            _notificacionService.AgregarNotificacion(
                $"La votación '{votacion.Nombre}' ha sido pausada temporalmente.",
                "Pausada",
                votacion
            );

            return Task.CompletedTask;
        }

        public Task OnVotacionDetenidaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación detenida: {VotacionId} - Nombre: {Nombre}",
                votacion.Id, votacion.Nombre);

            _notificacionService.AgregarNotificacion(
                $"La votación '{votacion.Nombre}' ha sido finalizada/detenida.",
                "Detenida",
                votacion
            );

            return Task.CompletedTask;
        }

        public Task OnVotacionAbiertaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación reabierta/reanudada: {VotacionId} - Nombre: {Nombre}",
                votacion.Id, votacion.Nombre);

            _notificacionService.AgregarNotificacion(
                $"La votación '{votacion.Nombre}' se encuentra ahora abierta para recibir votos.",
                "Abierta",
                votacion
            );

            return Task.CompletedTask;
        }
    }
}
