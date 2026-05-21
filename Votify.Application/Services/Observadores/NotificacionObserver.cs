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

        public NotificacionObserver(ILogger<NotificacionObserver> logger)
        {
            _logger = logger;
        }

        public Task OnVotacionCreadaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación creada: {VotacionId} - Nombre: {Nombre} - Tipo: {Tipo}",
                votacion.Id, votacion.Nombre, votacion.Tipo);

            // Aquí se puede implementar: enviar email, push notification, etc.
            // await _notificacionService.EnviarNotificacionCreadaAsync(votacion);

            return Task.CompletedTask;
        }

        public Task OnVotacionPausadaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación pausada: {VotacionId} - Nombre: {Nombre}",
                votacion.Id, votacion.Nombre);

            // Aquí se puede implementar: enviar notificación de pausa, etc.
            // await _notificacionService.EnviarNotificacionPausaAsync(votacion);

            return Task.CompletedTask;
        }

        public Task OnVotacionDetenidaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación detenida: {VotacionId} - Nombre: {Nombre}",
                votacion.Id, votacion.Nombre);

            // Aquí se puede implementar: enviar notificación de detención, etc.
            // await _notificacionService.EnviarNotificacionDetenciaAsync(votacion);

            return Task.CompletedTask;
        }

        public Task OnVotacionAbiertaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "Votación reabierta/reanudada: {VotacionId} - Nombre: {Nombre}",
                votacion.Id, votacion.Nombre);

            // Aquí se puede implementar: enviar notificación de reapertura, etc.
            // await _notificacionService.EnviarNotificacionAperturaAsync(votacion);

            return Task.CompletedTask;
        }
    }
}
