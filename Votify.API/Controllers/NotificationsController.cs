using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificationsController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpGet]
        public ActionResult<List<NotificationDto>> GetNotifications()
        {
            var notifications = _notificacionService.ObtenerNotificaciones();
            return Ok(notifications);
        }

        [HttpPost("read")]
        public IActionResult MarkAsRead()
        {
            _notificacionService.MarcarComoLeidas();
            return NoContent();
        }

        [HttpPost("clear")]
        public IActionResult ClearNotifications()
        {
            _notificacionService.LimpiarNotificaciones();
            return NoContent();
        }
    }
}
