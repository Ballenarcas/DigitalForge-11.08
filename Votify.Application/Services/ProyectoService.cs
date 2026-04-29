using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;
using Votify.Domain.Entities;

namespace Votify.Application.Services;

public class ProyectoService : IProyectoService
{
    private readonly IProyectoRepository _proyectoRepository;

    public ProyectoService(IProyectoRepository proyectoRepository)
    {
        _proyectoRepository = proyectoRepository;
    }

    public async Task<string> CrearProyectoAsync(ProyectoDto dto)
    {
        var proyecto = new Proyecto(dto.Nombre, dto.Descripcion, dto.Equipo_Id, dto.VotacionId, dto.ImagenUrl);
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