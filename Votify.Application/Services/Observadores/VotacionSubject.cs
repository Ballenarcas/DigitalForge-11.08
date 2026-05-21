using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services.Observadores
{
    /// <summary>
    /// Implementación concreta del Subject en el Patrón Observer.
    /// Mantiene la lista de observadores y notifica a todos cuando
    /// ocurre una transición de estado en una votación.
    /// 
    /// Si un observador lanza una excepción, se captura y se registra
    /// en el log sin interrumpir la notificación a los demás observadores.
    /// </summary>
    public class VotacionSubject : IVotacionObservable
    {
        private readonly List<IVotacionObserver> _observadores;
        private readonly ILogger<VotacionSubject> _logger;

        public VotacionSubject(
            IEnumerable<IVotacionObserver> observadores,
            ILogger<VotacionSubject> logger)
        {
            _observadores = new List<IVotacionObserver>(observadores);
            _logger = logger;
        }

        public void AgregarObservador(IVotacionObserver observador)
        {
            if (observador is null)
                throw new ArgumentNullException(nameof(observador));

            if (!_observadores.Contains(observador))
            {
                _observadores.Add(observador);
            }
        }

        public void RemoverObservador(IVotacionObserver observador)
        {
            _observadores.Remove(observador);
        }

        public async Task NotificarVotacionCreadaAsync(Votacion votacion)
        {
            foreach (var observador in _observadores)
            {
                try
                {
                    await observador.OnVotacionCreadaAsync(votacion);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error al notificar observador {ObservadorType} sobre creación de votación {VotacionId}",
                        observador.GetType().Name, votacion.Id);
                }
            }
        }

        public async Task NotificarVotacionPausadaAsync(Votacion votacion)
        {
            foreach (var observador in _observadores)
            {
                try
                {
                    await observador.OnVotacionPausadaAsync(votacion);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error al notificar observador {ObservadorType} sobre pausa de votación {VotacionId}",
                        observador.GetType().Name, votacion.Id);
                }
            }
        }

        public async Task NotificarVotacionDetenidaAsync(Votacion votacion)
        {
            foreach (var observador in _observadores)
            {
                try
                {
                    await observador.OnVotacionDetenidaAsync(votacion);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error al notificar observador {ObservadorType} sobre detención de votación {VotacionId}",
                        observador.GetType().Name, votacion.Id);
                }
            }
        }

        public async Task NotificarVotacionAbiertaAsync(Votacion votacion)
        {
            foreach (var observador in _observadores)
            {
                try
                {
                    await observador.OnVotacionAbiertaAsync(votacion);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error al notificar observador {ObservadorType} sobre apertura de votación {VotacionId}",
                        observador.GetType().Name, votacion.Id);
                }
            }
        }
    }
}
