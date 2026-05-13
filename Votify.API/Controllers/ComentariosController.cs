using Microsoft.AspNetCore.Authorization;
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
        private readonly IProyectoFachada _fachada;

        public ComentariosController(IProyectoFachada fachada)
        {
            _fachada = fachada;
        }

        [HttpPost]
        public async Task<IActionResult> AgregarComentario(string proyectoId, [FromBody] CrearComentarioRequest request)
        {
            try
            {
                await _fachada.AgregarComentarioAsync(proyectoId, request.Texto, request.AutorId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Hubo un error al guardar el comentario.", Detalle = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ComentarioDto>>> ObtenerComentarios(string proyectoId, [FromQuery] string? votacionId = null)
        {
            var usuarioId = ObtenerUsuarioId();
            if (string.IsNullOrWhiteSpace(usuarioId) || !Guid.TryParse(usuarioId, out var participanteId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }

            try
            {
                var comentarios = await _fachada.ObtenerComentariosAsync(proyectoId, participanteId, votacionId);
                return Ok(comentarios);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
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
