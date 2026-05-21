using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface INotificacionService
    {
        Task<Guid> CrearNotificacionAsync(Guid usuarioId, string mensaje, string tipo, string? recursoId = null, string? recursoTipo = null);
        Task<List<NotificacionDto>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task MarcarComoLeidaAsync(Guid notificacionId);
        Task<int> ObtenerNoLeidasCountAsync(Guid usuarioId);
    }
}