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
        private readonly IVotoService _votoService;

        public VotoController(IVotoService votoService)
        {
            _votoService = votoService;
        }

        [HttpPost]
        public async Task<IActionResult> Votar([FromBody] VotarDto request)
        {
            try
            {
                // Delegamos la validación y creación del voto a nuestro servicio
                await _votoService.VotarAsync(request);
                
                // Si todo va bien devolvemos un 200 OK con un mensajito o un 204 No Content
                return Ok(new { Mensaje = "Voto registrado con éxito." });
            }
            catch (InvalidOperationException ex)
            {
                // Si se dispara el límite de votos, devolvemos un HTTP 400 Bad Request
                return BadRequest(new { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Si mandan un VotacionId que no existe
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                // Cualquier otro error inesperado, incluyendo la InnerException que da Entity Framework
                return StatusCode(500, new { 
                    Error = "Ocurrió un error inesperado al procesar el voto.", 
                    Detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message 
                });
            }
        }
    }
}
