using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services.Observadores
{
    public class NotificacionSubject : INotificacionObservable
    {
        private readonly List<INotificacionObserver> _observadores;
        private readonly ILogger<NotificacionSubject> _logger;

        public NotificacionSubject(
            IEnumerable<INotificacionObserver> observadores,
            ILogger<NotificacionSubject> logger)
        {
            _observadores = new List<INotificacionObserver>(observadores);
            _logger = logger;
        }

        public void AgregarObservador(INotificacionObserver observador)
        {
            if (observador is null)
                throw new ArgumentNullException(nameof(observador));

            if (!_observadores.Contains(observador))
            {
                _observadores.Add(observador);
            }
        }

        public void RemoverObservador(INotificacionObserver observador)
        {
            _observadores.Remove(observador);
        }

        public async Task NotificarEquipoCreadoAsync(Equipo equipo, string nombreEvento)
        {
            foreach (var observador in _observadores)
            {
                try
                {
                    await observador.OnEquipoCreadoAsync(equipo, nombreEvento);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error al notificar observador {ObservadorType} sobre creación de equipo {EquipoId}",
                        observador.GetType().Name, equipo.Id);
                }
            }
        }

        public async Task NotificarProyectoCreadoAsync(Proyecto proyecto, string nombreVotacion)
        {
            foreach (var observador in _observadores)
            {
                try
                {
                    await observador.OnProyectoCreadoAsync(proyecto, nombreVotacion);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error al notificar observador {ObservadorType} sobre creación de proyecto {ProyectoId}",
                        observador.GetType().Name, proyecto.Id);
                }
            }
        }
    }
}