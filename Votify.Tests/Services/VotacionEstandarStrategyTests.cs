using Moq;
using Xunit;
using Votify.Application.DTOs;
using Votify.Application.Services.Estrategia;
using Votify.Domain.Entities;
using Votify.Domain.Factory;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Votify.Tests.Services
{
    public class VotacionEstandarStrategyTests
    {
        private readonly Mock<IVotoRepository> _mockVotoRepository;
        private readonly Mock<IProyectoRepository> _mockProyectoRepository;
        private readonly Mock<IEquipoRepository> _mockEquipoRepository;
        private readonly VotacionEstandarStrategy _strategy;

        public VotacionEstandarStrategyTests()
        {
            _mockVotoRepository = new Mock<IVotoRepository>();
            _mockProyectoRepository = new Mock<IProyectoRepository>();
            _mockEquipoRepository = new Mock<IEquipoRepository>();

            _strategy = new VotacionEstandarStrategy(
                _mockVotoRepository.Object,
                _mockProyectoRepository.Object,
                _mockEquipoRepository.Object
            );
        }

        [Fact]
        public async Task ProcesarVotoAsync_WithValidData_ShouldSaveVoto()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var votanteId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();

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

            var dto = new VotarDto
            {
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = votanteId
            };

            _mockVotoRepository.Setup(x => x.ContarVotosPorUsuarioYVotacionAsync(votacionId, votanteId)).ReturnsAsync(0);
            _mockVotoRepository.Setup(x => x.HaVotadoPorProyectoAsync(votacionId, proyectoId, votanteId)).ReturnsAsync(false);

            // Act
            await _strategy.ProcesarVotoAsync(votacion, dto);

            // Assert
            _mockVotoRepository.Verify(x => x.GuardarAsync(It.IsAny<Voto>()), Times.Once);
        }

        [Fact]
        public async Task ProcesarVotoAsync_WithExceededVoteLimit_ShouldThrowException()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
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
                Guid.NewGuid()
            );
            votacion.Id = Guid.Parse(votacionId);

            var dto = new VotarDto
            {
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = votanteId
            };

            _mockVotoRepository.Setup(x => x.ContarVotosPorUsuarioYVotacionAsync(votacionId, votanteId)).ReturnsAsync(limiteProy);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _strategy.ProcesarVotoAsync(votacion, dto));
            Assert.Contains("No puedes votar", exception.Message);
        }

        [Fact]
        public async Task ProcesarVotoAsync_WithDuplicateVote_ShouldThrowException()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var votanteId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();

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

            var dto = new VotarDto
            {
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = votanteId
            };

            _mockVotoRepository.Setup(x => x.ContarVotosPorUsuarioYVotacionAsync(votacionId, votanteId)).ReturnsAsync(0);
            _mockVotoRepository.Setup(x => x.HaVotadoPorProyectoAsync(votacionId, proyectoId, votanteId)).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _strategy.ProcesarVotoAsync(votacion, dto));
            Assert.Contains("Ya has votado", exception.Message);
        }

        [Fact]
        public async Task ProcesarVotoAsync_WithAnonymousVoter_ShouldSaveAnonimoVoto()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();

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

            var dto = new VotarDto
            {
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = null
            };

            _mockVotoRepository.Setup(x => x.ContarVotosPorUsuarioYVotacionAsync(votacionId, string.Empty)).ReturnsAsync(0);

            // Act
            await _strategy.ProcesarVotoAsync(votacion, dto);

            // Assert
            _mockVotoRepository.Verify(x => x.GuardarAsync(It.Is<Voto>(v => v is VotoAnonimo)), Times.Once);
        }

        [Fact]
        public void Tipo_ShouldReturnEstandar()
        {
            Assert.Equal("ESTANDAR", _strategy.Tipo);
        }

        [Fact]
        public async Task ProcesarVotoMulticriterioAsync_ShouldThrowNotSupported()
        {
            var votacion = new VotacionEstandar("Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), 3, false, false, Guid.NewGuid());
            await Assert.ThrowsAsync<NotSupportedException>(() => _strategy.ProcesarVotoMulticriterioAsync(votacion, new VotoMulticriterioDto()));
        }

        [Fact]
        public async Task CalcularResultadosAsync_WithNoVotes_ShouldReturnEmptyList()
        {
            var votacionId = Guid.NewGuid().ToString();
            _mockVotoRepository.Setup(x => x.ObtenerVotosPorVotacionAsync(votacionId)).ReturnsAsync(new List<(string, int)>());

            var result = await _strategy.CalcularResultadosAsync(votacionId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task CalcularResultadosAsync_WithVotes_ShouldReturnOrderedResults()
        {
            var votacionId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();
            var equipoId = Guid.NewGuid();

            _mockVotoRepository.Setup(x => x.ObtenerVotosPorVotacionAsync(votacionId))
                .ReturnsAsync(new List<(string ProyectoId, int Votos)> { (proyectoId, 5) });
            _mockProyectoRepository.Setup(x => x.ObtenerPorVotacionAsync(votacionId))
                .ReturnsAsync(new List<Proyecto> { new Proyecto("Proyecto A", "Desc", equipoId.ToString(), Guid.Parse(votacionId), id: proyectoId) });
            _mockEquipoRepository.Setup(x => x.ObtenerTodosAsync())
                .ReturnsAsync(new List<Equipo> { new Equipo("Equipo A") { Id = equipoId } });

            var result = await _strategy.CalcularResultadosAsync(votacionId);

            Assert.Single(result);
            Assert.Equal(proyectoId, result[0].Id);
            Assert.Equal(5, result[0].TotalVotos);
            Assert.Equal(1, result[0].Posicion);
        }
    }
}
