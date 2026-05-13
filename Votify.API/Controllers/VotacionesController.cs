using Microsoft.AspNetCore.Mvc;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/votaciones")]
    public class VotacionesController : ControllerBase
    {
        private readonly IVotacionFachada _fachada;

        public VotacionesController(IVotacionFachada fachada)
        {
            _fachada = fachada;
        }

        [HttpPost]
        public async Task<IActionResult> CrearVotacionn([FromBody] CrearVotacionDto dto)
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
    }
}