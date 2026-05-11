using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/participantes")]
    public class ParticipantesController : ControllerBase
    {
        private readonly IParticipanteRepository _participanteRepository;

        public ParticipantesController(IParticipanteRepository participanteRepository)
        {
            _participanteRepository = participanteRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParticipanteDto>>> ObtenerParticipantes()
        {
            var participantes = await _participanteRepository.ObtenerTodosAsync();
            var dtos = participantes.Select(p => new ParticipanteDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Email = p.Email,
                EquipoId = p.EquipoId
            });
            return Ok(dtos);
        }
    }

}
