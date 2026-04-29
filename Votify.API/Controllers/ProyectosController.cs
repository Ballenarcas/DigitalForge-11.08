using Microsoft.AspNetCore.Mvc;
using Votify.API.DTOs;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/proyectos")]
    public class ProyectosController : ControllerBase
    {
        private readonly IProyectoService _proyectoService;

        public ProyectosController(IProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CrearProyecto([FromBody] ProyectoDto dto)
        {
            try
            {
                var id = await _proyectoService.CrearProyectoAsync(dto);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message} | StackTrace: {ex.StackTrace}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProyectoDto>> ObtenerProyecto(string id)
        {
            var proyecto = await _proyectoService.ObtenerProyectoAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return Ok(proyecto);
        }

        public async Task<ActionResult<List<ProyectoDto>>> ObtenerProyectos()
        {
            var proyectos = await _proyectoService.ObtenerProyectosAsync();
            return Ok(proyectos);
        }

        [HttpGet("votacion/{votacionId}")]
        public async Task<ActionResult<List<ProyectoDto>>> ObtenerProyectosPorVotacion(string votacionId)
        {
            var proyectos = await _proyectoService.ObtenerProyectosPorVotacionAsync(votacionId);
            return Ok(proyectos);
        }
    }
}