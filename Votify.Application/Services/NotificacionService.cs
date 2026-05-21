using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _repository;

        public NotificacionService(INotificacionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CrearNotificacionAsync(Guid usuarioId, string mensaje, string tipo, string? recursoId = null, string? recursoTipo = null)
        {
            var notificacion = new Notificacion(usuarioId, mensaje, tipo, recursoId, recursoTipo);
            await _repository.GuardarAsync(notificacion);
            return notificacion.Id;
        }

        public async Task<List<NotificacionDto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            var notificaciones = await _repository.ObtenerPorUsuarioAsync(usuarioId);
            return notificaciones.Select(n => new NotificacionDto
            {
                Id = n.Id,
                UsuarioId = n.UsuarioId,
                Mensaje = n.Mensaje,
                Tipo = n.Tipo,
                RecursoId = n.RecursoId,
                RecursoTipo = n.RecursoTipo,
                Leida = n.Leida,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        public async Task MarcarComoLeidaAsync(Guid notificacionId)
        {
            await _repository.MarcarComoLeidaAsync(notificacionId);
        }

        public async Task<int> ObtenerNoLeidasCountAsync(Guid usuarioId)
        {
            return await _repository.ObtenerNoLeidasCountAsync(usuarioId);
        }
    }
}