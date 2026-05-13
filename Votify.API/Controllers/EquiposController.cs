using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/equipos")]
    public class EquiposController : ControllerBase
    {
        private readonly IEquipoFachada _fachada;

        public EquiposController(IEquipoFachada fachada)
        {
            _fachada = fachada;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EquipoDto>> CrearEquipo([FromBody] CrearEquipoRequest request)
        {
            try
            {
                var equipo = await _fachada.CrearEquipoAsync(request.Nombre);
                return Ok(equipo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost("{equipoId}/miembros")]
        [Authorize]
        public async Task<ActionResult> AsignarParticipante(Guid equipoId, [FromBody] AsignarMiembroRequest request)
        {
            try
            {
                var solicitanteId = ObtenerUsuarioId();
                if (string.IsNullOrWhiteSpace(solicitanteId) || !Guid.TryParse(solicitanteId, out var solicitanteGuid))
                {
                    return Unauthorized(new { Message = "Usuario no autenticado." });
                }

                await _fachada.AsignarParticipanteAsync(solicitanteGuid, request.ParticipanteId, equipoId, request.EventoId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<EquipoDto>>> ObtenerEquipos()
        {
            var equipos = await _fachada.ObtenerTodosAsync();
            return Ok(equipos);
        }

        [HttpGet("participante/{participanteId}")]
        public async Task<ActionResult<EquipoDto>> ObtenerEquipoDeParticipante(Guid participanteId)
        {
            var equipo = await _fachada.ObtenerEquipoDeParticipanteAsync(participanteId);
            if (equipo == null)
            {
                return NotFound("El participante no tiene un equipo asignado.");
            }
            return Ok(equipo);
        }

        private string? ObtenerUsuarioId()
        {
            return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value
                   ?? User.FindFirst("nameid")?.Value;
        }
    }


}
