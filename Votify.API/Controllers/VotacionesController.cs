using Microsoft.AspNetCore.Mvc;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/votaciones")]
    public class VotacionesController : ControllerBase
    {
        private readonly IVotacionService _service;

        public VotacionesController(IVotacionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CrearVotacionn([FromBody] CrearVotacionDto dto)
        {
            await _service.CrearVotacionAsync(dto);

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
        [HttpGet]
        public async Task<ActionResult<List<CrearVotacionResponse>>> Get()
        {
            var votaciones = await _service.ObtenerTodasAsync();
            return Ok(votaciones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CrearVotacionResponse>> GetById(string id)
        {
            var votacion = await _service.ObtenerPorIdAsync(id);
            if (votacion is null) return NotFound();
            return Ok(votacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(string id, [FromBody] CrearVotacionDto dto)
        {
            await _service.ActualizarVotacionAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(string id)
        {
            try
            {
                await _service.EliminarVotacionAsync(id);
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
            var resultados = await _service.ObtenerResultadosAsync(id);
            return Ok(resultados);
        }
    }
}