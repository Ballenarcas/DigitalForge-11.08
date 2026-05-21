using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface INotificacionRepository
    {
        Task GuardarAsync(Notificacion notificacion);
        Task<List<Notificacion>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task<Notificacion?> ObtenerPorIdAsync(Guid id);
        Task MarcarComoLeidaAsync(Guid id);
        Task<int> ObtenerNoLeidasCountAsync(Guid usuarioId);
    }
}