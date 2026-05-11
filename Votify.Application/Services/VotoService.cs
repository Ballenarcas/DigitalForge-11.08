using System;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;
using Votify.Domain.Factory;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class VotoService : IVotoService
    {
        private readonly IVotoRepository _votoRepository;
        private readonly IVotacionRepository _votacionRepository;
        private readonly IParticipanteEventoRepository _participanteEventoRepository;
        private readonly ICriterioRepository? _criterioRepository;
        private readonly IValoracionCriterioRepository? _valoracionCriterioRepository;
        private readonly IComentarioRepository? _comentarioRepository;

        public VotoService(
            IVotoRepository votoRepository,
            IVotacionRepository votacionRepository,
            IParticipanteEventoRepository participanteEventoRepository,
            ICriterioRepository? criterioRepository = null,
            IValoracionCriterioRepository? valoracionCriterioRepository = null,
            IComentarioRepository? comentarioRepository = null)
        {
            _votoRepository = votoRepository;
            _votacionRepository = votacionRepository;
            _participanteEventoRepository = participanteEventoRepository;
            _criterioRepository = criterioRepository;
            _valoracionCriterioRepository = valoracionCriterioRepository;
            _comentarioRepository = comentarioRepository;
        }

        public async Task VotarAsync(VotarDto dto)
        {
            var votacion = await _votacionRepository.ObtenerAsync(dto.VotacionId);
            var EventoId = await _votacionRepository.ObtenerEventoIdAsync(dto.VotacionId);
            
            if (votacion == null)
            {
                throw new ArgumentException("La votación especificada no existe.");
            }

            // Validar de acuerdo al estado y línea de tiempo usando el Patrón Estado
            votacion.ValidarVoto();

            int votosActuales = await _votoRepository.ContarVotosPorUsuarioYVotacionAsync(dto.VotacionId, dto.VotanteId ?? string.Empty);

            if (votosActuales >= votacion.LimiteProy)
            {
                throw new InvalidOperationException($"No puedes votar. Has alcanzado el límite de {votacion.LimiteProy} votos para esta votación.");
            }
            if (await _participanteEventoRepository.ObtenerRolAsync(Guid.Parse(EventoId), Guid.Parse(dto.VotanteId)) == "ORGANIZADOR")
            {
                throw new InvalidOperationException("Los organizadores no pueden votar en sus propios eventos.");
            }
            if (!string.IsNullOrEmpty(dto.VotanteId))
            {
                bool haVotado = await _votoRepository.HaVotadoPorProyectoAsync(dto.VotacionId, dto.ProyectoId, dto.VotanteId);
                if (haVotado)
                {
                    throw new InvalidOperationException("Ya has votado por este proyecto en esta votacion.");
                }
            }

            VotoFactory factory;
            if (string.IsNullOrEmpty(dto.VotanteId))
            {
                factory = new VotoAnonimoFactory();
            }
            else
            {
                factory = new VotoEstandarFactory();
            }

            var nuevoVoto = factory.Crear(dto.ProyectoId, dto.VotacionId, dto.VotanteId);

            await _votoRepository.GuardarAsync(nuevoVoto);
        }

        public async Task VotarMulticriterioAsync(VotoMulticriterioDto dto)
        {
            if (_criterioRepository is null || _valoracionCriterioRepository is null || _comentarioRepository is null)
            {
                throw new InvalidOperationException("El servicio de evaluación multicriterio no está configurado.");
            }

            var votacion = await _votacionRepository.ObtenerAsync(dto.VotacionId);
            var eventoId = await _votacionRepository.ObtenerEventoIdAsync(dto.VotacionId);

            if (votacion == null)
            {
                throw new ArgumentException("La votación especificada no existe.");
            }

            if (!EsMulticriterio(votacion.Tipo))
            {
                throw new InvalidOperationException("Esta votación no está configurada como Multicriterio.");
            }

            votacion.ValidarVoto();

            if (string.IsNullOrWhiteSpace(dto.Comentario))
            {
                throw new InvalidOperationException("El comentario es obligatorio en una votación multicriterio.");
            }

            if (await _valoracionCriterioRepository.HaValoradoProyectoAsync(dto.ProyectoId, dto.VotanteId))
            {
                throw new InvalidOperationException("El voto ya ha sido emitido.");
            }

            var rol = await _participanteEventoRepository.ObtenerRolAsync(Guid.Parse(eventoId!), Guid.Parse(dto.VotanteId));
            if (!string.Equals(rol?.Trim(), "Jurado", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Solo el rol Jurado puede evaluar esta votación multicriterio.");
            }

            var criterios = await _criterioRepository.ObtenerPorVotacionAsync(dto.VotacionId);
            if (!criterios.Any())
            {
                throw new InvalidOperationException("La votación multicriterio no tiene criterios configurados.");
            }

            if (dto.Valoraciones.Count != criterios.Count)
            {
                throw new InvalidOperationException("Debes valorar todos los criterios.");
            }

            var criterioIds = criterios.Select(c => c.Id).ToHashSet();
            var valoraciones = dto.Valoraciones.Select(v =>
            {
                if (!Guid.TryParse(v.CriterioId, out var criterioId) || !criterioIds.Contains(criterioId))
                {
                    throw new InvalidOperationException("Uno de los criterios no pertenece a esta votación.");
                }

                if (v.Valoracion < 1 || v.Valoracion > 5)
                {
                    throw new InvalidOperationException("Las valoraciones multicriterio deben estar entre 1 y 5.");
                }

                return new ValoracionCriterio
                {
                    CriterioId = criterioId,
                    Valoracion = v.Valoracion
                };
            }).ToList();

            await _valoracionCriterioRepository.GuardarAsync(dto.ProyectoId, dto.VotanteId, valoraciones);
            await _comentarioRepository.GuardarAsync(dto.ProyectoId, dto.Comentario, Guid.Parse(dto.VotanteId));
        }

        public async Task<bool> PuedeVotarAsync(string votacionId, string votanteId)
        {
            var votacion = await _votacionRepository.ObtenerAsync(votacionId);
            if (votacion == null) return false;

            int votosActuales = await _votoRepository.ContarVotosPorUsuarioYVotacionAsync(votacionId, votanteId);
            return votosActuales < votacion.LimiteProy;
        }

        public async Task<bool> HaVotadoMulticriterioAsync(string proyectoId, string votanteId)
        {
            if (_valoracionCriterioRepository is null)
            {
                return false;
            }

            return await _valoracionCriterioRepository.HaValoradoProyectoAsync(proyectoId, votanteId);
        }

        private static bool EsMulticriterio(string? tipo)
        {
            return string.Equals(tipo?.Trim(), "Multicriterio", StringComparison.OrdinalIgnoreCase);
        }

    }
}
