using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _repo;
        private readonly IParticipanteEventoRepository _participanteEventoRepo;
        private readonly IVotacionService _votacionService;

        public EventoService(IEventoRepository repo, IParticipanteEventoRepository participanteEventoRepo, IVotacionService votacionService)
        {
            _repo = repo;
            _participanteEventoRepo = participanteEventoRepo;
            _votacionService = votacionService;
        }

        public async Task<List<EventoDto>> ObtenerTodosAsync()
        {
            var entidades = await _repo.ObtenerTodosAsync();

            return entidades.Select(MapToEventoDTO).ToList();
        }

        public async Task<List<EventoDto>> ObtenerMisEventosAsync(string participanteId)
        {
            if (!Guid.TryParse(participanteId, out var pId))
            {
                return new List<EventoDto>();
            }

            var entidades = await _repo.ObtenerPorParticipanteAsync(pId);

            return entidades.Select(MapToEventoDTO).ToList();
        }

        public async Task<EventoDto?> ObtenerPorIdAsync(string id)
        {
            var e = await _repo.ObtenerPorIdAsync(id);
            if (e is null) return null;

            return MapToEventoDTO(e);
        }

        public async Task<EventoDto> CrearAsync(EventoDto dto, string creadorId)
        {
            var e = new Evento(
                dto.Nombre,
                dto.Descripcion,
                dto.FechaInicio,
                dto.FechaFin,
                dto.ImagenUrl);

            await _repo.GuardarAsync(e);

            if (Guid.TryParse(creadorId, out var participanteId))
            {
                var pe = new ParticipanteEvento(participanteId, e.Id, "ORGANIZADOR");
                await _participanteEventoRepo.GuardarAsync(pe);
            }

            if (dto.Categorias?.Any() == true)
            {
                var categoriasUnicas = dto.Categorias
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => c.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var categoria in categoriasUnicas)
                {
                    var votacionDto = new CrearVotacionDto
                    {
                        Nombre = categoria,
                        Tipo = "ESTANDAR",
                        FechaInicio = dto.FechaInicio,
                        FechaFin = dto.FechaFin,
                        LimiteProy = 1,
                        Comentarios = false,
                        ComentariosObligatorios = false,
                        EsAnonima = false,
                        EventoId = e.Id.ToString(),
                        ImagenUrl = dto.ImagenUrl
                    };

                    await _votacionService.CrearVotacionAsync(votacionDto);
                }
            }

            dto.Id = e.Id.ToString();
            return dto;
        }

        public async Task<EventoDto> ActualizarAsync(EventoDto dto, string solicitanteId)
        {
            if (!Guid.TryParse(dto.Id, out var eventoId))
            {
                throw new ArgumentException("ID de evento invalido");
            }

            if (!Guid.TryParse(solicitanteId, out var solicitanteGuid))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            await ValidarOrganizadorAsync(eventoId, solicitanteGuid);

            var actualizado = await _repo.ActualizarEventoAsync(eventoId, dto.Nombre, dto.Descripcion, dto.FechaInicio, dto.FechaFin, dto.ImagenUrl);
            if (!actualizado)
            {
                throw new InvalidOperationException("Evento no encontrado");
            }

            dto.FechaInicio = dto.FechaInicio;
            dto.FechaFin = dto.FechaFin;
            return dto;
        }

        public async Task RegistrarParticipanteAsync(string eventoId, string participanteId)
        {
            if (Guid.TryParse(eventoId, out var eId) && Guid.TryParse(participanteId, out var pId))
            {
                var misEventos = await _repo.ObtenerPorParticipanteAsync(pId);
                if (!misEventos.Any(x => x.Id == eId))
                {
                    var pe = new ParticipanteEvento(pId, eId, "PÚBLICO");
                    await _participanteEventoRepo.GuardarAsync(pe);
                }
            }
        }

        public async Task<List<ParticipanteRolDto>> ObtenerParticipantesPorEventoAsync(string eventoId, string solicitanteId, string? search = null)
        {
            if (!Guid.TryParse(eventoId, out var eId) || !Guid.TryParse(solicitanteId, out var solicitanteGuid))
            {
                return new List<ParticipanteRolDto>();
            }

            await ValidarOrganizadorAsync(eId, solicitanteGuid);

            var participantes = await _participanteEventoRepo.ObtenerParticipantesPorEventoAsync(eId, search);
            return participantes.Select(p => new ParticipanteRolDto
            {
                Id = p.ParticipanteId.ToString(),
                Nombre = p.Nombre,
                Email = p.Email,
                Rol = NormalizarRol(p.Rol)
            }).ToList();
        }

        public async Task<RoleStatisticsDto> ObtenerEstadisticasRolesAsync(string eventoId, string solicitanteId)
        {
            if (!Guid.TryParse(eventoId, out var eId) || !Guid.TryParse(solicitanteId, out var solicitanteGuid))
            {
                return new RoleStatisticsDto();
            }

            await ValidarOrganizadorAsync(eId, solicitanteGuid);

            var stats = await _participanteEventoRepo.ContarRolesPorEventoAsync(eId);
            return new RoleStatisticsDto
            {
                Organizadores = stats.Organizadores,
                Jurados = stats.Jurados,
                Competidores = stats.Competidores,
                Publicos = stats.Publicos
            };
        }

        public async Task CambiarRolParticipanteAsync(string eventoId, string participanteId, string solicitanteId, string rol)
        {
            if (Guid.TryParse(eventoId, out var eId) && Guid.TryParse(participanteId, out var pId) && Guid.TryParse(solicitanteId, out var solicitanteGuid))
            {
                await ValidarOrganizadorAsync(eId, solicitanteGuid);

                if (pId == solicitanteGuid)
                {
                    throw new InvalidOperationException("No puedes cambiar tu propio rol");
                }

                var actualizado = await _participanteEventoRepo.ActualizarRolAsync(eId, pId, NormalizarRol(rol));
                if (!actualizado)
                {
                    throw new InvalidOperationException("No se pudo actualizar el rol del participante.");
                }
            }
        }

        public async Task EliminarParticipacionAsync(string eventoId, string participanteId, string solicitanteId)
        {
            if (Guid.TryParse(eventoId, out var eId) && Guid.TryParse(participanteId, out var pId) && Guid.TryParse(solicitanteId, out var solicitanteGuid))
            {
                await ValidarOrganizadorAsync(eId, solicitanteGuid);

                var totalConRol = await _participanteEventoRepo.ContarParticipantesConRolAsync(eId);
                if (totalConRol <= 1)
                {
                    throw new InvalidOperationException("Debe haber al menos un participante con rol");
                }

                if (pId == solicitanteGuid)
                {
                    throw new InvalidOperationException("No puedes cambiar tu propio rol");
                }

                var eliminado = await _participanteEventoRepo.EliminarAsync(eId, pId);
                if (!eliminado)
                {
                    throw new InvalidOperationException("No se pudo quitar al participante del evento.");
                }
            }
        }

        public async Task EliminarEventoAsync(string eventoId, string solicitanteId)
        {
            if (!Guid.TryParse(eventoId, out var eId) || !Guid.TryParse(solicitanteId, out var solicitanteGuid))
            {
                throw new ArgumentException("ID de evento invalido");
            }

            await ValidarOrganizadorAsync(eId, solicitanteGuid);

            var eliminado = await _repo.EliminarAsync(eventoId);
            if (!eliminado)
            {
                throw new InvalidOperationException("No se pudo eliminar el evento.");
            }
        }

        public async Task<string?> ObtenerRolEnEventoAsync(string eventoId, string participanteId)
        {
            if (Guid.TryParse(eventoId, out var eId) && Guid.TryParse(participanteId, out var pId))
            {
                return await _participanteEventoRepo.ObtenerRolAsync(eId, pId);
            }
            return null;
        }

        public async Task<Dictionary<string, string>> ObtenerMisRolesAsync(string participanteId)
        {
            if (!Guid.TryParse(participanteId, out var pId))
                return new Dictionary<string, string>();

            var roles = await _participanteEventoRepo.ObtenerRolesPorParticipanteAsync(pId);
            return roles.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
        }

        private static EventoDto MapToEventoDTO(Evento e) => new EventoDto
        {
            Id          = e.Id.ToString(),
            Nombre      = e.Nombre,
            Descripcion = e.Descripcion,
            FechaInicio = e.FechaInicio,
            FechaFin    = e.FechaFin,
            ImagenUrl   = e.ImagenUrl
        };
        private async Task ValidarOrganizadorAsync(Guid eventoId, Guid participanteId)
        {
            var rol = await _participanteEventoRepo.ObtenerRolAsync(eventoId, participanteId);
            if (!EsOrganizador(rol))
            {
                throw new UnauthorizedAccessException("No tienes permisos");
            }
        }

        private static bool EsOrganizador(string? rol)
        {
            return string.Equals(rol?.Trim(), "ORGANIZADOR", StringComparison.OrdinalIgnoreCase)
                || string.Equals(rol?.Trim(), "Organizador", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizarRol(string rol)
        {
            return rol.Trim().ToUpperInvariant() switch
            {
                "ORGANIZADOR" => "ORGANIZADOR",
                "JURADO" => "JURADO",
                "COMPETIDOR" => "COMPETIDOR",
                "PUBLICO" => "PÚBLICO",
                "PÚBLICO" => "PÚBLICO",
                _ => rol.Trim().ToUpperInvariant()
            };
        }
    }
}
