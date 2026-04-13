using Microsoft.AspNetCore.Mvc;
using Votify.API.DTOs;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/proyectos/{proyectoId}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly IComentarioService _comentarioService;

        public ComentariosController(IComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        [HttpPost]
        public async Task<IActionResult> AgregarComentario(string proyectoId, [FromBody] CrearComentarioRequest request)
        {
            try
            {
                await _comentarioService.AgregarComentarioAsync(proyectoId, request.Texto, request.AutorId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                // Manejo genérico de errores (loguear idealmente)
                return StatusCode(500, new { Error = "Hubo un error al guardar el comentario.", Detalle = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDto>>> ObtenerComentarios(string proyectoId, [FromQuery] string? votacionId = null)
        {
            var comentarios = await _comentarioService.ObtenerComentariosAsync(proyectoId, votacionId);
            return Ok(comentarios);
        }
    }
}
