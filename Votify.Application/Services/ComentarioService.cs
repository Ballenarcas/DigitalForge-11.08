using System;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IVotacionRepository _votacionRepository;

        public ComentarioService(IComentarioRepository comentarioRepository, IVotacionRepository votacionRepository)
        {
            _comentarioRepository = comentarioRepository;
            _votacionRepository = votacionRepository;
        }

        public async Task AgregarComentarioAsync(string proyectoId, string texto, Guid? autorId = null)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                throw new ArgumentException("El comentario no puede estar vacío.");
            }

            await _comentarioRepository.GuardarAsync(proyectoId, texto, autorId);
        }

        public async Task<List<ComentarioDto>> ObtenerComentariosAsync(string proyectoId, string? votacionId = null)
        {
            var comentarios = await _comentarioRepository.ObtenerAsync(proyectoId);
            var esVotacionAnonima = false;

            if (!string.IsNullOrEmpty(votacionId))
            {
                var votacion = await _votacionRepository.ObtenerAsync(votacionId);
                esVotacionAnonima = votacion?.EsAnonima ?? false;
            }

            return comentarios.Select(c => new ComentarioDto
            {
                Texto = c.Texto,
                AutorId = esVotacionAnonima ? null : c.AutorId,
                EsAnonimo = esVotacionAnonima || !c.AutorId.HasValue,
                FechaCreacion = c.FechaCreacion
            }).ToList();
        }
    }
}
