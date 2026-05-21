using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services.Observadores
{
    /// <summary>
    /// Observador que registra datos analíticos sobre el ciclo de vida de las votaciones.
    /// Puede ser utilizado para reportes, dashboards, y análisis de comportamiento.
    /// </summary>
    public class AnalyticsObserver : IVotacionObserver
    {
        private readonly ILogger<AnalyticsObserver> _logger;

        public AnalyticsObserver(ILogger<AnalyticsObserver> logger)
        {
            _logger = logger;
        }

        public Task OnVotacionCreadaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "ANALYTICS: Nueva votación - Evento: {EventoId}, Tipo: {Tipo}, Timestamp: {Timestamp}",
                votacion.EventoId, votacion.Tipo, DateTime.UtcNow);

            // Aquí se puede implementar: registrar en base de datos analítica, enviar a Segment/Mixpanel, etc.
            // await _analyticsService.RegistrarEventoAsync("votacion_creada", new { votacion.Id, votacion.Tipo, votacion.EventoId });

            return Task.CompletedTask;
        }

        public Task OnVotacionPausadaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "ANALYTICS: Votación pausada - ID: {VotacionId}, Timestamp: {Timestamp}",
                votacion.Id, DateTime.UtcNow);

            // Aquí se puede implementar: registrar evento analítico de pausa, etc.
            // await _analyticsService.RegistrarEventoAsync("votacion_pausada", new { votacion.Id });

            return Task.CompletedTask;
        }

        public Task OnVotacionDetenidaAsync(Votacion votacion)
        {
            var tiempoVida = DateTime.UtcNow - votacion.FechaInicio;

            _logger.LogInformation(
                "ANALYTICS: Votación detenida - ID: {VotacionId}, TiempoVida: {TiempoVida}, Timestamp: {Timestamp}",
                votacion.Id, tiempoVida, DateTime.UtcNow);

            // Aquí se puede implementar: registrar evento de cierre con duración total, etc.
            // await _analyticsService.RegistrarEventoAsync("votacion_detenida", new { votacion.Id, tiempoVida });

            return Task.CompletedTask;
        }

        public Task OnVotacionAbiertaAsync(Votacion votacion)
        {
            _logger.LogInformation(
                "ANALYTICS: Votación reabierta - ID: {VotacionId}, Timestamp: {Timestamp}",
                votacion.Id, DateTime.UtcNow);

            // Aquí se puede implementar: registrar evento analítico de reapertura, etc.
            // await _analyticsService.RegistrarEventoAsync("votacion_reabierta", new { votacion.Id });

            return Task.CompletedTask;
        }
    }
}
