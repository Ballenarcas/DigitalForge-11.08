using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Application.Services;
using Votify.Domain.Entities;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/equipos")]
    public class EquiposController : ControllerBase
    {
        private readonly EquipoService _equipoService;

        public EquiposController(EquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Equipo>> CrearEquipo([FromBody] CrearEquipoRequest request)
        {
            try
            {
                var equipo = await _equipoService.CrearEquipoAsync(request.Nombre);
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

                await _equipoService.AsignarParticipanteAEquipoAsync(solicitanteGuid, request.ParticipanteId, equipoId, request.EventoId);
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
        public async Task<ActionResult<IEnumerable<Equipo>>> ObtenerEquipos()
        {
            var equipos = await _equipoService.ObtenerTodosLosEquiposAsync();
            return Ok(equipos);
        }

        [HttpGet("participante/{participanteId}")]
        public async Task<ActionResult<Equipo>> ObtenerEquipoDeParticipante(Guid participanteId)
        {
            var equipo = await _equipoService.ObtenerEquipoDeParticipanteAsync(participanteId);
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
