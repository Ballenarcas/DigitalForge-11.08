using System.Collections.Generic;
using Votify.Application.DTOs;
using Votify.Domain.Entities;

namespace Votify.Application.Interfaces
{
    public interface INotificacionService
    {
        void AgregarNotificacion(string mensaje, string tipoEvento, Votacion votacion);
        List<NotificationDto> ObtenerNotificaciones();
        void MarcarComoLeidas();
        void LimpiarNotificaciones();
    }
}
