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

        public VotoService(IVotoRepository votoRepository, IVotacionRepository votacionRepository, IParticipanteEventoRepository participanteEventoRepository)
        {
            _votoRepository = votoRepository;
            _votacionRepository = votacionRepository;
            _participanteEventoRepository = participanteEventoRepository;
        }

        public async Task VotarAsync(VotarDto dto)
        {
            var votacion = await _votacionRepository.ObtenerAsync(dto.VotacionId);
            var EventoId = await _votacionRepository.ObtenerEventoIdAsync(dto.VotacionId);
            
            if (votacion == null)
            {
                throw new ArgumentException("La votación especificada no existe.");
            }

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
        public async Task<bool> PuedeVotarAsync(string votacionId, string votanteId)
        {
            var votacion = await _votacionRepository.ObtenerAsync(votacionId);
            if (votacion == null) return false;

            int votosActuales = await _votoRepository.ContarVotosPorUsuarioYVotacionAsync(votacionId, votanteId);
            return votosActuales < votacion.LimiteProy;
        }
    }
}
