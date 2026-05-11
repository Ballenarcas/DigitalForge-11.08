using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> AsignarParticipante(Guid equipoId, [FromBody] AsignarMiembroRequest request)
        {
            try
            {
                await _equipoService.AsignarParticipanteAEquipoAsync(request.ParticipanteId, equipoId, request.EventoId);
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
    }


}
