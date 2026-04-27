using Microsoft.AspNetCore.Mvc;
using Votify.Application.Interfaces;
using Votify.Application.DTOs;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/eventos")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _service;

        public EventosController(IEventoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventos = await _service.ObtenerTodosAsync();
            return Ok(eventos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var evento = await _service.ObtenerPorIdAsync(id);
            if (evento is null) return NotFound();
            return Ok(evento);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventoDto dto)
        {
            var result = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
    }
}
