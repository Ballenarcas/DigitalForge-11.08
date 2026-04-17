using System;
using System.Threading.Tasks;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Factory;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class VotoService : IVotoService
    {
        private readonly IVotoRepository _votoRepository;
        private readonly IVotacionRepository _votacionRepository;

        public VotoService(IVotoRepository votoRepository, IVotacionRepository votacionRepository)
        {
            _votoRepository = votoRepository;
            _votacionRepository = votacionRepository;
        }

        public async Task VotarAsync(VotarDto dto)
        {
            var votacion = await _votacionRepository.ObtenerAsync(dto.VotacionId);
            
            if (votacion == null)
            {
                throw new ArgumentException("La votación especificada no existe.");
            }

            int votosActuales = await _votoRepository.ContarVotosPorUsuarioYVotacionAsync(dto.VotacionId, dto.VotanteId ?? string.Empty);

            if (votosActuales >= votacion.LimiteProyectos)
            {
                throw new InvalidOperationException($"No puedes votar. Has alcanzado el límite de {votacion.LimiteProyectos} votos para esta votación.");
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
            return votosActuales < votacion.LimiteProyectos;
        }
    }
}
