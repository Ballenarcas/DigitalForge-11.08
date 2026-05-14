using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/votos")]
    public class VotoController : ControllerBase
    {
        private readonly IVotoFachada _fachada;

        public VotoController(IVotoFachada fachada)
        {
            _fachada = fachada;
        }

        [HttpPost]
        public async Task<IActionResult> Votar([FromBody] VotarDto request)
        {
            try
            {
                await _fachada.VotarAsync(request);
                return Ok(new { Mensaje = "Voto registrado con éxito." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Error = "Ocurrió un error inesperado al procesar el voto.", 
                    Detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message 
                });
            }
        }

        [HttpPost("multicriterio")]
        public async Task<IActionResult> VotarMulticriterio([FromBody] VotoMulticriterioDto request)
        {
            try
            {
                await _fachada.VotarMulticriterioAsync(request);
                return Ok(new { Mensaje = "Voto registrado con éxito." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    Error = "Ocurrió un error inesperado al procesar el voto multicriterio.",
                    Detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message
                });
            }
        }

        [HttpPost("multicriterio-publico")]
        public async Task<IActionResult> VotarMulticriterioPublico([FromBody] VotoMulticriterioAnonimoDto request)
        {
            try
            {
                await _fachada.VotarMulticriterioAnonimoAsync(request);
                return Ok(new { Mensaje = "Voto público registrado con éxito." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    Error = "Ocurrió un error inesperado al procesar el voto multicriterio público.",
                    Detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message
                });
            }
        }

        [HttpGet("puede-votar/{votacionId}/{votanteId}")]
        public async Task<IActionResult> PuedeVotar(string votacionId, string votanteId)
        {
            try
            {
                bool puedeVotar = await _fachada.PuedeVotarAsync(votacionId, votanteId);
                return Ok(puedeVotar);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("multicriterio/emitido/{proyectoId}/{votanteId}")]
        public async Task<IActionResult> HaVotadoMulticriterio(string proyectoId, string votanteId)
        {
            try
            {
                bool emitido = await _fachada.HaVotadoMulticriterioAsync(proyectoId, votanteId);
                return Ok(emitido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
