using Moq;
using Xunit;
using Votify.Application.DTOs;
using Votify.Application.Services;
using Votify.Tests.Builders;
using Votify.Application.Services.Estrategia;
using Votify.Domain.Entities;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;
using System;

namespace Votify.Tests.Services
{
    public class VotoServiceTests
    {
        private readonly Mock<IVotoRepository> _mockVotoRepository;
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IParticipanteEventoRepository> _mockParticipanteEventoRepository;
        private readonly Mock<IVotacionStrategy> _mockEstandarStrategy;
        private readonly VotacionStrategyResolver _strategyResolver;
        private readonly VotoService _votoService;

        public VotoServiceTests()
        {
            _mockVotoRepository = new Mock<IVotoRepository>();
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockParticipanteEventoRepository = new Mock<IParticipanteEventoRepository>();
            _mockEstandarStrategy = new Mock<IVotacionStrategy>();
            _mockEstandarStrategy.Setup(s => s.Tipo).Returns("ESTANDAR");

            _strategyResolver = new VotacionStrategyResolver(new[] { _mockEstandarStrategy.Object });

            _votoService = new VotoService(
                _mockVotoRepository.Object,
                _mockVotacionRepository.Object,
                _mockParticipanteEventoRepository.Object,
                _strategyResolver
            );
        }

        #region VotarAsync Tests

        [Fact]
        public async Task VotarAsync_WithValidData_ShouldDelegateToStrategy()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var eventoId = Guid.NewGuid().ToString();
            var votanteId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();

            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1),
                3,
                false,
                false,
                Guid.Parse(eventoId)
            );
            votacion.Id = Guid.Parse(votacionId);

            var dto = new VotarDto
            {
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = votanteId
            };

            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId)).ReturnsAsync(votacion);
            _mockVotacionRepository.Setup(x => x.ObtenerEventoIdAsync(votacionId)).ReturnsAsync(eventoId);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(Guid.Parse(eventoId), Guid.Parse(votanteId))).ReturnsAsync("PARTICIPANTE");
            _mockEstandarStrategy.Setup(x => x.ProcesarVotoAsync(It.IsAny<Votacion>(), It.IsAny<VotarDto>())).Returns(Task.CompletedTask);

            // Act
            await _votoService.VotarAsync(dto);

            // Assert
            _mockEstandarStrategy.Verify(x => x.ProcesarVotoAsync(votacion, dto), Times.Once);
        }

        [Fact]
        public async Task VotarAsync_WithNonexistentVotacion_ShouldThrowException()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var dto = new VotarDto
            {
                VotacionId = votacionId,
                ProyectoId = Guid.NewGuid().ToString(),
                VotanteId = Guid.NewGuid().ToString()
            };

            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId)).ReturnsAsync((Votacion?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _votoService.VotarAsync(dto));
        }

        [Fact]
        public async Task VotarAsync_WithExceededVoteLimit_ShouldThrowException_FromStrategy()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var eventoId = Guid.NewGuid().ToString();
            var votanteId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();
            var limiteProy = 2;

            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1),
                limiteProy,
                false,
                false,
                Guid.Parse(eventoId)
            );
            votacion.Id = Guid.Parse(votacionId);

            var dto = new VotarDto
            {
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = votanteId
            };

            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId)).ReturnsAsync(votacion);
            _mockVotacionRepository.Setup(x => x.ObtenerEventoIdAsync(votacionId)).ReturnsAsync(eventoId);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(Guid.Parse(eventoId), Guid.Parse(votanteId))).ReturnsAsync("PARTICIPANTE");
            _mockEstandarStrategy.Setup(x => x.ProcesarVotoAsync(It.IsAny<Votacion>(), It.IsAny<VotarDto>()))
                .ThrowsAsync(new InvalidOperationException($"No puedes votar. Has alcanzado el límite de {limiteProy} votos para esta votación."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _votoService.VotarAsync(dto));
            Assert.Contains("No puedes votar", exception.Message);
        }

        [Fact]
        public async Task VotarAsync_WithOrganizerUser_ShouldThrowException()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var eventoId = Guid.NewGuid().ToString();
            var votanteId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();

            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1),
                3,
                false,
                false,
                Guid.Parse(eventoId)
            );
            votacion.Id = Guid.Parse(votacionId);

            var dto = new VotarDto
            {
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = votanteId
            };

            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId)).ReturnsAsync(votacion);
            _mockVotacionRepository.Setup(x => x.ObtenerEventoIdAsync(votacionId)).ReturnsAsync(eventoId);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(Guid.Parse(eventoId), Guid.Parse(votanteId))).ReturnsAsync("ORGANIZADOR");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _votoService.VotarAsync(dto));
            Assert.Contains("organizadores no pueden votar", exception.Message);
        }

        [Fact]
        public async Task VotarAsync_WithDuplicateVote_ShouldThrowException_FromStrategy()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var eventoId = Guid.NewGuid().ToString();
            var votanteId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();

            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1),
                3,
                false,
                false,
                Guid.Parse(eventoId)
            );
            votacion.Id = Guid.Parse(votacionId);

            var dto = new VotarDto
            {
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = votanteId
            };

            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId)).ReturnsAsync(votacion);
            _mockVotacionRepository.Setup(x => x.ObtenerEventoIdAsync(votacionId)).ReturnsAsync(eventoId);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(Guid.Parse(eventoId), Guid.Parse(votanteId))).ReturnsAsync("PARTICIPANTE");
            _mockEstandarStrategy.Setup(x => x.ProcesarVotoAsync(It.IsAny<Votacion>(), It.IsAny<VotarDto>()))
                .ThrowsAsync(new InvalidOperationException("Ya has votado por este proyecto en esta votacion."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _votoService.VotarAsync(dto));
            Assert.Contains("Ya has votado", exception.Message);
        }

        [Fact]
        public async Task VotarAsync_WithAnonymousVoter_ShouldDelegateToStrategy()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var eventoId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();

            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1),
                3,
                false,
                false,
                Guid.Parse(eventoId)
            );
            votacion.Id = Guid.Parse(votacionId);

            var dto = new VotarDto
            {
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = null
            };

            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId)).ReturnsAsync(votacion);
            _mockVotacionRepository.Setup(x => x.ObtenerEventoIdAsync(votacionId)).ReturnsAsync(eventoId);
            _mockEstandarStrategy.Setup(x => x.ProcesarVotoAsync(It.IsAny<Votacion>(), It.IsAny<VotarDto>())).Returns(Task.CompletedTask);

            // Act
            await _votoService.VotarAsync(dto);

            // Assert
            _mockEstandarStrategy.Verify(x => x.ProcesarVotoAsync(votacion, dto), Times.Once);
        }

        #endregion

        #region PuedeVotarAsync Tests

        [Fact]
        public async Task PuedeVotarAsync_WithValidVotation_ShouldReturnTrue()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var votanteId = Guid.NewGuid().ToString();

            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1),
                3,
                false,
                false,
                Guid.NewGuid()
            );
            votacion.Id = Guid.Parse(votacionId);

            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId)).ReturnsAsync(votacion);
            _mockVotoRepository.Setup(x => x.ContarVotosPorUsuarioYVotacionAsync(votacionId, votanteId)).ReturnsAsync(1);

            // Act
            var result = await _votoService.PuedeVotarAsync(votacionId, votanteId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PuedeVotarAsync_WithExceededLimit_ShouldReturnFalse()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var votanteId = Guid.NewGuid().ToString();

            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1),
                2,
                false,
                false,
                Guid.NewGuid()
            );
            votacion.Id = Guid.Parse(votacionId);

            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId)).ReturnsAsync(votacion);
            _mockVotoRepository.Setup(x => x.ContarVotosPorUsuarioYVotacionAsync(votacionId, votanteId)).ReturnsAsync(2);

            // Act
            var result = await _votoService.PuedeVotarAsync(votacionId, votanteId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PuedeVotarAsync_WithNonexistentVotation_ShouldReturnFalse()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var votanteId = Guid.NewGuid().ToString();

            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId)).ReturnsAsync((Votacion?)null);

            // Act
            var result = await _votoService.PuedeVotarAsync(votacionId, votanteId);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
