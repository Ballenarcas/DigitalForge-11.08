using Microsoft.AspNetCore.Mvc;
using Votify.Domain.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly ILogger<FilesController> _logger;

        public FilesController(IStorageService storageService, ILogger<FilesController> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string bucket = "proyectos")
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (bucket != "Eventos" && 
                !bucket.Equals("Proyectos", StringComparison.OrdinalIgnoreCase) && 
                !bucket.Equals("Votaciones", StringComparison.OrdinalIgnoreCase))
                bucket = "Proyectos";

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            try
            {
                using var stream = file.OpenReadStream();
                var url = await _storageService.SubirArchivoAsync(bucket, stream, fileName, file.ContentType);
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subiendo archivo a Supabase Storage");
                return StatusCode(500, new { Error = "Error al subir la imagen.", Detalle = ex.Message });
            }
        }
    }
}