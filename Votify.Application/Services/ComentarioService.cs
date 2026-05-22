using System;
using System.Linq;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IProyectoRepository _proyectoRepository;
        private readonly IVotacionRepository _votacionRepository;
        private readonly IParticipanteRepository _participanteRepository;
        private readonly IParticipanteEventoRepository _participanteEventoRepository;

        public ComentarioService(
            IComentarioRepository comentarioRepository,
            IProyectoRepository proyectoRepository,
            IVotacionRepository votacionRepository,
            IParticipanteRepository participanteRepository,
            IParticipanteEventoRepository participanteEventoRepository)
        {
            _comentarioRepository = comentarioRepository;
            _proyectoRepository = proyectoRepository;
            _votacionRepository = votacionRepository;
            _participanteRepository = participanteRepository;
            _participanteEventoRepository = participanteEventoRepository;
        }

        public async Task AgregarComentarioAsync(string proyectoId, string texto, Guid? autorId = null)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                throw new ArgumentException("El comentario no puede estar vacío.");
            }

            if (autorId.HasValue)
            {
                bool haComentado = await _comentarioRepository.HaComentadoProyectoAsync(proyectoId, autorId.Value);
                if (haComentado)
                {
                    throw new InvalidOperationException("Solo puedes dejar un comentario por proyecto.");
                }
            }

            await _comentarioRepository.GuardarAsync(proyectoId, texto, autorId);
        }

        public async Task<List<ComentarioDto>> ObtenerComentariosAsync(string proyectoId, Guid usuarioId, string? votacionId = null)
        {
            var proyecto = await _proyectoRepository.ObtenerAsync(proyectoId);
            if (proyecto == null)
            {
                return new List<ComentarioDto>();
            }

            if (!await PuedeVerComentariosAsync(proyecto, usuarioId))
            {
                throw new UnauthorizedAccessException("No tienes permisos para ver los comentarios de este proyecto.");
            }

            return await MapearComentariosAsync(proyectoId, proyecto);
        }

        public async Task<List<ComentarioDto>> ObtenerComentariosParaResumenAsync(string proyectoId)
        {
            var proyecto = await _proyectoRepository.ObtenerAsync(proyectoId);
            if (proyecto == null)
            {
                return new List<ComentarioDto>();
            }

            return await MapearComentariosAsync(proyectoId, proyecto);
        }

        private async Task<List<ComentarioDto>> MapearComentariosAsync(string proyectoId, Proyecto proyecto)
        {
            var comentarios = await _comentarioRepository.ObtenerAsync(proyectoId);
            var esVotacionAnonima = false;

            if (Guid.TryParse(proyecto.VotacionId.ToString(), out var votacionGuid))
            {
                var votacion = await _votacionRepository.ObtenerAsync(votacionGuid.ToString());
                esVotacionAnonima = votacion?.EsAnonima ?? false;
            }

            return comentarios.Select(c => new ComentarioDto
            {
                Texto = c.Texto,
                AutorId = esVotacionAnonima ? null : c.AutorId,
                AutorNombre = esVotacionAnonima ? null : c.AutorNombre,
                EsAnonimo = esVotacionAnonima || !c.AutorId.HasValue,
                FechaCreacion = c.FechaCreacion
            }).ToList();
        }

        private async Task<bool> PuedeVerComentariosAsync(Proyecto proyecto, Guid usuarioId)
        {
            if (!Guid.TryParse(proyecto.Equipo_Id, out var equipoId))
            {
                return false;
            }

            var participante = await _participanteRepository.ObtenerPorIdAsync(usuarioId);
            if (participante?.EquipoId == equipoId)
            {
                return true;
            }

            var votacion = await _votacionRepository.ObtenerAsync(proyecto.VotacionId.ToString());
            if (votacion == null)
            {
                return false;
            }

            var rol = await _participanteEventoRepository.ObtenerRolAsync(votacion.EventoId, usuarioId);
            return EsOrganizador(rol);
        }

        private static bool EsOrganizador(string? rol) =>
            string.Equals(rol?.Trim(), "ORGANIZADOR", StringComparison.OrdinalIgnoreCase)
            || string.Equals(rol?.Trim(), "Organizador", StringComparison.OrdinalIgnoreCase);
    }
}
