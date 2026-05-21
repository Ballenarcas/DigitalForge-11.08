using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services.Observadores
{
    /// <summary>
    /// Observador que monitorea el estado de las votaciones para scoring/análisis.
    /// Puede registrar métricas sobre el tiempo de vida de las votaciones, transiciones de estado, etc.
    /// </summary>
    public class ScoringObserver : IVotacionObserver
    {
        private readonly ILogger<ScoringObserver> _logger;

        public ScoringObserver(ILogger<ScoringObserver> logger)
        {
            _logger = logger;
        }

        public Task OnVotacionCreadaAsync(Votacion votacion)
        {
            _logger.LogDebug(
                "SCORING: Votación creada - ID: {VotacionId}, Límite: {Limite}, Anónima: {EsAnonima}",
                votacion.Id, votacion.LimiteProy, votacion.EsAnonima);

            // Aquí se puede implementar: registrar métrica de creación, puntuación inicial, etc.
            // await _metricsService.RecordarMetricaAsync("votacion.creada", votacion);

            return Task.CompletedTask;
        }

        public Task OnVotacionPausadaAsync(Votacion votacion)
        {
            var duracion = votacion.FechaFin - votacion.FechaInicio;

            _logger.LogDebug(
                "SCORING: Votación pausada - ID: {VotacionId}, Duración planeada: {Duracion}",
                votacion.Id, duracion);

            // Aquí se puede implementar: registrar métrica de pausa, etc.
            // await _metricsService.RecordarMetricaAsync("votacion.pausada", votacion);

            return Task.CompletedTask;
        }

        public Task OnVotacionDetenidaAsync(Votacion votacion)
        {
            _logger.LogDebug(
                "SCORING: Votación detenida - ID: {VotacionId}",
                votacion.Id);

            // Aquí se puede implementar: registrar métrica final de votación, calcular score, etc.
            // await _metricsService.CalcularScoreFinalAsync(votacion);

            return Task.CompletedTask;
        }

        public Task OnVotacionAbiertaAsync(Votacion votacion)
        {
            _logger.LogDebug(
                "SCORING: Votación reabierta - ID: {VotacionId}",
                votacion.Id);

            // Aquí se puede implementar: registrar métrica de reapertura, etc.
            // await _metricsService.RecordarMetricaAsync("votacion.reabierta", votacion);

            return Task.CompletedTask;
        }
    }
}
