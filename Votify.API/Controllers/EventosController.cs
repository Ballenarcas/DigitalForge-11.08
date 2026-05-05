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
        private readonly ILogger<EventosController> _logger;

        public EventosController(IEventoService service, ILogger<EventosController> logger)
        {
            _service = service;
            _logger = logger;
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
            var usuarioId = ObtenerUsuarioId();
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
            var usuarioId = ObtenerUsuarioId();
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
            var usuarioId = ObtenerUsuarioId();
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
            var usuarioId = ObtenerUsuarioId();
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

        [HttpGet("{id}/participantes")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetParticipantes(string id, [FromQuery] string? search)
        {
            var usuarioId = ObtenerUsuarioId();
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }

            try
            {
                var participantes = await _service.ObtenerParticipantesPorEventoAsync(id, usuarioId, search);
                return Ok(participantes);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Intento no autorizado de gestión de roles. Evento: {EventoId}, Usuario: {UsuarioId}", id, usuarioId);
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
            }
        }

        [HttpGet("{id}/roles/count")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetRoleCount(string id)
        {
            var usuarioId = ObtenerUsuarioId();
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }

            try
            {
                var stats = await _service.ObtenerEstadisticasRolesAsync(id, usuarioId);
                return Ok(stats);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Intento no autorizado de conteo de roles. Evento: {EventoId}, Usuario: {UsuarioId}", id, usuarioId);
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
            }
        }

        [HttpPut("{id}/participantes/{participanteId}/rol")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UpdateRol(string id, string participanteId, [FromBody] ActualizarRolDto dto)
        {
            var usuarioId = ObtenerUsuarioId();
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }

            if (string.IsNullOrWhiteSpace(dto?.Rol))
            {
                return BadRequest(new { Message = "El rol es requerido." });
            }

            try
            {
                await _service.CambiarRolParticipanteAsync(id, participanteId, usuarioId, dto.Rol);
                return Ok(new { Message = "Rol cambiado exitosamente" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Intento no autorizado de cambio de rol. Evento: {EventoId}, Usuario: {UsuarioId}, Participante: {ParticipanteId}", id, usuarioId, participanteId);
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}/participantes/{participanteId}")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> DeleteParticipante(string id, string participanteId)
        {
            var usuarioId = ObtenerUsuarioId();
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }

            try
            {
                await _service.EliminarParticipacionAsync(id, participanteId, usuarioId);
                return Ok(new { Message = "Participante quitado correctamente" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Intento no autorizado de eliminación de participante. Evento: {EventoId}, Usuario: {UsuarioId}, Participante: {ParticipanteId}", id, usuarioId, participanteId);
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        private string? ObtenerUsuarioId()
        {
            return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value
                   ?? User.FindFirst("nameid")?.Value;
        }
    }
}
