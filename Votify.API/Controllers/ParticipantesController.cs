using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/participantes")]
    public class ParticipantesController : ControllerBase
    {
        private readonly IParticipanteFachada _fachada;

        public ParticipantesController(IParticipanteFachada fachada)
        {
            _fachada = fachada;
        }

        [HttpGet]
        public async Task<ActionResult<List<ParticipanteDto>>> ObtenerParticipantes()
        {
            var participantes = await _fachada.ObtenerTodosAsync();
            return Ok(participantes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParticipanteDto>> ObtenerParticipantePorId(Guid id)
        {
            var participante = await _fachada.ObtenerPorIdAsync(id);
            if (participante == null)
                return NotFound();
            return Ok(participante);
        }
    }

}
