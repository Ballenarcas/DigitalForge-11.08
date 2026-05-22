using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using System.Security.Claims;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/votaciones")]
    public class VotacionesController : ControllerBase
    {
        private readonly IVotacionFachada _fachada;
        private readonly IManualVotosService _manualVotosService;
        private readonly IEventoService _eventoService;

        public VotacionesController(
            IVotacionFachada fachada,
            IManualVotosService manualVotosService,
            IEventoService eventoService)
        {
            _fachada = fachada;
            _manualVotosService = manualVotosService;
            _eventoService = eventoService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearVotacion([FromBody] CrearVotacionDto dto)
        {
            try
            {
                await _fachada.CrearVotacionAsync(dto);
                return Ok(new
                {
                    dto.Nombre,
                    dto.Tipo,
                    dto.FechaInicio,
                    dto.FechaFin,
                    dto.LimiteProy,
                    dto.Comentarios
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<CrearVotacionResponse>>> Get()
        {
            var votaciones = await _fachada.ObtenerVotacionesAsync();
            return Ok(votaciones);
        }
        [HttpGet("evento/{eventoId}")]
        public async Task<ActionResult<List<CrearVotacionResponse>>> GetByEvento(string eventoId)
        {
            var votaciones = await _fachada.ObtenerVotacionesPorEventoAsync(eventoId);
            return Ok(votaciones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CrearVotacionResponse>> GetById(string id)
        {
            var votacion = await _fachada.ObtenerVotacionAsync(id);
            if (votacion is null) return NotFound();
            return Ok(votacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(string id, [FromBody] CrearVotacionDto dto)
        {
            try
            {
                await _fachada.ActualizarVotacionAsync(id, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(string id)
        {
            try
            {
                await _fachada.EliminarVotacionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/resultados")]
        public async Task<ActionResult<List<ResultadoProyectoDto>>> ObtenerResultados(string id)
        {
            var resultados = await _fachada.ObtenerResultadosAsync(id);
            return Ok(resultados);
        }

        [HttpGet("{id}/resultados-multicriterio")]
        public async Task<ActionResult<List<ResultadoMulticriterioDto>>> ObtenerResultadosMulticriterio(string id)
        {
            var resultados = await _fachada.ObtenerResultadosMulticriterioAsync(id);
            return Ok(resultados);
        }

        [HttpPatch("{id}/pausar")]
        public async Task<IActionResult> PausarVotacion(string id)
        {
            try
            {
                await _fachada.PausarVotacionAsync(id);
                return Ok(new { mensaje = "La votación ha sido pausada exitosamente." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}/detener")]
        public async Task<IActionResult> DetenerVotacion(string id)
        {
            try
            {
                await _fachada.DetenerVotacionAsync(id);
                return Ok(new { mensaje = "La votación ha sido detenida exitosamente." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}/abrir")]
        public async Task<IActionResult> AbrirVotacion(string id)
        {
            try
            {
                await _fachada.AbrirVotacionAsync(id);
                return Ok(new { mensaje = "La votación ha sido abierta exitosamente." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/resultados/asignar")]
        [Authorize]
        public async Task<IActionResult> AsignarResultadoManual(string id, [FromBody] AsignacionManualVotosDto dto)
        {
            try
            {
                var votacion = await _fachada.ObtenerVotacionAsync(id);
                if (votacion == null)
                    return NotFound(new { error = "Votación no encontrada" });

                var participanteId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value
                    ?? User.FindFirst("nameid")?.Value;

                if (string.IsNullOrEmpty(participanteId))
                    return Unauthorized();

                // Validar que el usuario sea ORGANIZADOR del evento
                var rol = await _eventoService.ObtenerRolEnEventoAsync(votacion.EventoId, participanteId);
                if (!IsOrganizador(rol))
                    return Forbid("Solo los ORGANIZADORES pueden asignar resultados manualmente");

                await _manualVotosService.GuardarAsignacionManualAsync(id, participanteId, dto);
                return Ok(new { mensaje = "Asignación manual guardada correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}/resultados/manuales")]
        [Authorize]
        public async Task<IActionResult> ObtenerAsignacionesManuales(string id)
        {
            try
            {
                var votacion = await _fachada.ObtenerVotacionAsync(id);
                if (votacion == null)
                    return NotFound(new { error = "Votación no encontrada" });

                var participanteId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value
                    ?? User.FindFirst("nameid")?.Value;

                if (string.IsNullOrEmpty(participanteId))
                    return Unauthorized();

                // Validar que el usuario sea ORGANIZADOR del evento
                var rol = await _eventoService.ObtenerRolEnEventoAsync(votacion.EventoId, participanteId);
                if (!IsOrganizador(rol))
                    return Forbid("Solo los ORGANIZADORES pueden ver asignaciones manuales");

                var asignaciones = await _manualVotosService.ObtenerAsignacionesManualesAsync(id);
                return Ok(asignaciones);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private static bool IsOrganizador(string? rol)
        {
            return string.Equals(rol?.Trim(), "ORGANIZADOR", StringComparison.OrdinalIgnoreCase)
                || string.Equals(rol?.Trim(), "Organizador", StringComparison.OrdinalIgnoreCase);
        }

        [HttpPost("{id}/resultados/justificacion")]
        [Authorize]
        public async Task<IActionResult> GuardarJustificacion(string id, [FromBody] GuardarJustificacionDto dto)
        {
            try
            {
                var votacion = await _fachada.ObtenerVotacionAsync(id);
                if (votacion == null)
                    return NotFound(new { error = "Votación no encontrada" });

                var participanteId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value
                    ?? User.FindFirst("nameid")?.Value;

                if (string.IsNullOrEmpty(participanteId))
                    return Unauthorized();

                var rol = await _eventoService.ObtenerRolEnEventoAsync(votacion.EventoId, participanteId);
                if (!IsOrganizador(rol))
                    return Forbid("Solo los ORGANIZADORES pueden agregar justificaciones");

                await _manualVotosService.GuardarJustificacionAsync(id, participanteId, dto);
                return Ok(new { mensaje = "Justificación guardada correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}/resultados/justificacion/{proyectoId}")]
        [Authorize]
        public async Task<IActionResult> ObtenerJustificacion(string id, string proyectoId)
        {
            try
            {
                var justificacion = await _manualVotosService.ObtenerJustificacionAsync(id, proyectoId);
                if (justificacion == null)
                    return Ok(new { });

                return Ok(justificacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}