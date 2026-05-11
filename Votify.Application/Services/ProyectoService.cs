using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;
using Votify.Domain.Entities;

namespace Votify.Application.Services;

public class ProyectoService : IProyectoService
{
    private readonly IProyectoRepository _proyectoRepository;
    private readonly IParticipanteRepository _participanteRepository;

    public ProyectoService(IProyectoRepository proyectoRepository, IParticipanteRepository participanteRepository)
    {
        _proyectoRepository = proyectoRepository;
        _participanteRepository = participanteRepository;
    }

    public async Task<string> CrearProyectoAsync(ProyectoDto dto)
    {
        if (dto.ParticipanteId == null)
            throw new ArgumentException("Se requiere el ID del participante para crear un proyecto.");

        var participante = await _participanteRepository.ObtenerPorIdAsync(dto.ParticipanteId.Value);
        if (participante == null)
            throw new ArgumentException("Participante no encontrado.");

        if (participante.EquipoId == null)
            throw new InvalidOperationException("Debes pertenecer a un equipo para crear un proyecto.");

        string equipoIdStr = participante.EquipoId.Value.ToString();

        var proyectosVotacion = await _proyectoRepository.ObtenerPorVotacionAsync(dto.VotacionId.ToString());
        if (proyectosVotacion.Any(p => p.Equipo_Id == equipoIdStr))
            throw new InvalidOperationException("Tu equipo ya ha creado un proyecto en esta votacin.");

        var proyecto = new Proyecto(dto.Nombre, dto.Descripcion, equipoIdStr, dto.VotacionId, dto.ImagenUrl);
        await _proyectoRepository.GuardarAsync(proyecto);
        return proyecto.Id;
    }

    public async Task<ProyectoDto?> ObtenerProyectoAsync(string id)
    {
        var proyecto = await _proyectoRepository.ObtenerAsync(id);
        if (proyecto == null)
        {
            return null;
        }
        return new ProyectoDto
        {
            Id = proyecto.Id,
            Nombre = proyecto.Nombre,
            Descripcion = proyecto.Descripcion,
            Equipo_Id = proyecto.Equipo_Id,
            VotacionId = proyecto.VotacionId,
            ImagenUrl = proyecto.ImagenUrl
        };
    }

    public async Task<List<ProyectoDto>> ObtenerProyectosAsync()
    {
        var proyectos = await _proyectoRepository.ObtenerTodasAsync();
        return proyectos.Select(p => new ProyectoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Equipo_Id = p.Equipo_Id,
            VotacionId = p.VotacionId,
            ImagenUrl = p.ImagenUrl
        }).ToList();
    }

    public async Task<List<ProyectoDto>> ObtenerProyectosPorVotacionAsync(string votacionId)
    {
        var proyectos = await _proyectoRepository.ObtenerPorVotacionAsync(votacionId);
        return proyectos.Select(p => new ProyectoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Equipo_Id = p.Equipo_Id,
            VotacionId = p.VotacionId,
            ImagenUrl = p.ImagenUrl
        }).ToList();
    }
}