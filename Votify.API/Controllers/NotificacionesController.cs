using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/notificaciones")]
    [Authorize]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionesController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        private Guid? ObtenerUsuarioId()
        {
            var idStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value
                       ?? User.FindFirst("nameid")?.Value;

            if (string.IsNullOrWhiteSpace(idStr) || !Guid.TryParse(idStr, out var id))
                return null;

            return id;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificacionDto>>> ObtenerNotificaciones()
        {
            var usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            var notificaciones = await _notificacionService.ObtenerPorUsuarioAsync(usuarioId.Value);
            return Ok(notificaciones);
        }

        [HttpGet("no-leidas")]
        public async Task<ActionResult<int>> ObtenerNoLeidasCount()
        {
            var usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            var count = await _notificacionService.ObtenerNoLeidasCountAsync(usuarioId.Value);
            return Ok(count);
        }

        [HttpPut("{id}/leer")]
        public async Task<ActionResult> MarcarComoLeida(Guid id)
        {
            var usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            await _notificacionService.MarcarComoLeidaAsync(id);
            return Ok();
        }
    }
}