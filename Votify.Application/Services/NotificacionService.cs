using System;
using System.Collections.Generic;
using System.Linq;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;

namespace Votify.Application.Services
{
    /// <summary>
    /// Servicio singleton en memoria para almacenar y gestionar las notificaciones
    /// generadas por el NotificacionObserver del ciclo de vida de las votaciones.
    /// </summary>
    public class NotificacionService : INotificacionService
    {
        private readonly List<NotificationDto> _notificaciones = new List<NotificationDto>();
        private readonly object _lock = new object();
        private const int MaxNotificaciones = 50;

        public void AgregarNotificacion(string mensaje, string tipoEvento, Votacion votacion)
        {
            lock (_lock)
            {
                var notificacion = new NotificationDto
                {
                    Id = Guid.NewGuid(),
                    Mensaje = mensaje,
                    Fecha = DateTime.UtcNow,
                    VotacionId = votacion.Id,
                    VotacionNombre = votacion.Nombre,
                    TipoEvento = tipoEvento,
                    Leido = false
                };

                _notificaciones.Insert(0, notificacion); // Las más recientes primero

                // Limitar el tamaño máximo de notificaciones en memoria
                while (_notificaciones.Count > MaxNotificaciones)
                {
                    _notificaciones.RemoveAt(_notificaciones.Count - 1);
                }
            }
        }

        public List<NotificationDto> ObtenerNotificaciones()
        {
            lock (_lock)
            {
                // Devolver copia para evitar problemas de concurrencia
                return _notificaciones.Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Mensaje = n.Mensaje,
                    Fecha = n.Fecha,
                    VotacionId = n.VotacionId,
                    VotacionNombre = n.VotacionNombre,
                    TipoEvento = n.TipoEvento,
                    Leido = n.Leido
                }).ToList();
            }
        }

        public void MarcarComoLeidas()
        {
            lock (_lock)
            {
                foreach (var notificacion in _notificaciones)
                {
                    notificacion.Leido = true;
                }
            }
        }

        public void LimpiarNotificaciones()
        {
            lock (_lock)
            {
                _notificaciones.Clear();
            }
        }
    }
}
