using Microsoft.AspNetCore.Mvc;
using Votify.Application.Interfaces;
using Votify.Application.DTOs;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/eventos")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _service;

        public EventosController(IEventoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventos = await _service.ObtenerTodosAsync();
            return Ok(eventos);
        }

        [HttpGet("mis-eventos")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetMisEventos()
        {
            var usuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value
                            ?? User.FindFirst("nameid")?.Value;
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }

            var eventos = await _service.ObtenerMisEventosAsync(usuarioId);
            return Ok(eventos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var evento = await _service.ObtenerPorIdAsync(id);
            if (evento is null) return NotFound();
            return Ok(evento);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Create([FromBody] EventoDto dto)
        {
            var usuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value
                            ?? User.FindFirst("nameid")?.Value;
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }

            var result = await _service.CrearAsync(dto, usuarioId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("{id}/participar")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Participar(string id)
        {
            var usuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value
                            ?? User.FindFirst("nameid")?.Value;
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }

            await _service.RegistrarParticipanteAsync(id, usuarioId);
            return Ok();
        }

        [HttpGet("{id}/rol")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetRol(string id)
        {
            var usuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value
                            ?? User.FindFirst("nameid")?.Value;
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }

            var rol = await _service.ObtenerRolEnEventoAsync(id, usuarioId);
            if (string.IsNullOrEmpty(rol))
            {
                return NotFound(new { Message = "El usuario no participa en este evento." });
            }

            return Ok(new { Rol = rol });
        }
    }
}
