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
            // 1. Obtener la votación para conocer su límite de proyectos
            var votacion = await _votacionRepository.ObtenerAsync(dto.VotacionId);
            
            if (votacion == null)
            {
                throw new ArgumentException("La votación especificada no existe.");
            }

            // 2. Contar los votos que ya tiene este usuario en esta votación específica
            int votosActuales = await _votoRepository.ContarVotosPorUsuarioYVotacionAsync(dto.VotacionId, dto.VotanteId ?? string.Empty);

            // 3. Validar si ya alcanzó o superó el límite
            if (votosActuales >= votacion.LimiteProyectos)
            {
                throw new InvalidOperationException($"No puedes votar. Has alcanzado el límite de {votacion.LimiteProyectos} votos para esta votación.");
            }

            // 4. Crear el voto usando la Factory según sea anónimo o estándar
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

            // 5. Guardar el voto de forma segura
            await _votoRepository.GuardarAsync(nuevoVoto);
        }
    }
}
