using System;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Application.Services.Estrategia;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class VotoService : IVotoService
    {
        private readonly IVotoRepository _votoRepository;
        private readonly IVotacionRepository _votacionRepository;
        private readonly IParticipanteEventoRepository _participanteEventoRepository;
        private readonly IValoracionCriterioRepository? _valoracionCriterioRepository;
        private readonly VotacionStrategyResolver _strategyResolver;

        public VotoService(
            IVotoRepository votoRepository,
            IVotacionRepository votacionRepository,
            IParticipanteEventoRepository participanteEventoRepository,
            VotacionStrategyResolver strategyResolver,
            IValoracionCriterioRepository? valoracionCriterioRepository = null)
        {
            _votoRepository = votoRepository;
            _votacionRepository = votacionRepository;
            _participanteEventoRepository = participanteEventoRepository;
            _strategyResolver = strategyResolver;
            _valoracionCriterioRepository = valoracionCriterioRepository;
        }

        public async Task VotarAsync(VotarDto dto)
        {
            var votacion = await _votacionRepository.ObtenerAsync(dto.VotacionId);
            if (votacion == null)
            {
                throw new ArgumentException("La votación especificada no existe.");
            }

            votacion.ValidarVoto();
            await ValidarNoEsOrganizadorAsync(dto.VotacionId, dto.VotanteId);

            var strategy = _strategyResolver.Resolver(votacion.Tipo);
            await strategy.ProcesarVotoAsync(votacion, dto);
        }

        public async Task VotarMulticriterioAsync(VotoMulticriterioDto dto)
        {
            var votacion = await _votacionRepository.ObtenerAsync(dto.VotacionId);
            if (votacion == null)
            {
                throw new ArgumentException("La votación especificada no existe.");
            }

            votacion.ValidarVoto();
            await ValidarNoEsOrganizadorAsync(dto.VotacionId, dto.VotanteId);

            var strategy = _strategyResolver.Resolver(votacion.Tipo);
            await strategy.ProcesarVotoMulticriterioAsync(votacion, dto);
        }

        public async Task VotarMulticriterioAnonimoAsync(VotoMulticriterioAnonimoDto dto)
        {
            var votacion = await _votacionRepository.ObtenerAsync(dto.VotacionId);
            if (votacion == null)
            {
                throw new ArgumentException("La votación especificada no existe.");
            }

            votacion.ValidarVoto();

            var strategy = _strategyResolver.Resolver(votacion.Tipo);
            await strategy.ProcesarVotoMulticriterioAnonimoAsync(votacion, dto);
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

        private async Task ValidarNoEsOrganizadorAsync(string votacionId, string? votanteId)
        {
            if (string.IsNullOrEmpty(votanteId))
                return;

            var eventoId = await _votacionRepository.ObtenerEventoIdAsync(votacionId);
            if (string.IsNullOrEmpty(eventoId))
                return;

            var rol = await _participanteEventoRepository.ObtenerRolAsync(Guid.Parse(eventoId), Guid.Parse(votanteId));
            if (rol == "ORGANIZADOR")
            {
                throw new InvalidOperationException("Los organizadores no pueden votar en sus propios eventos.");
            }
        }
    }
}
