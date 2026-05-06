using Moq;
using Xunit;
using Votify.Application.DTOs;
using Votify.Application.Services;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using System;

namespace Votify.Tests.Services
{
    public class VotacionServiceTests
    {
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IVotoRepository> _mockVotoRepository;
        private readonly Mock<IProyectoRepository> _mockProyectoRepository;
        private readonly Mock<IEventoRepository> _mockEventoRepository;
        private readonly VotacionService _votacionService;

        public VotacionServiceTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockVotoRepository = new Mock<IVotoRepository>();
            _mockProyectoRepository = new Mock<IProyectoRepository>();
            _mockEventoRepository = new Mock<IEventoRepository>();

            _votacionService = new VotacionService(
                _mockVotacionRepository.Object,
                _mockVotoRepository.Object,
                _mockProyectoRepository.Object,
                _mockEventoRepository.Object
            );
        }

        [Fact]
        public async Task CrearVotacionAsync_WithInvalidDateOrder_ShouldThrowException()
        {
            // Arrange
            var eventoId = Guid.NewGuid().ToString();
            var dto = new CrearVotacionDto
            {
                Nombre = "Votación Test",
                Tipo = "ESTANDAR",
                FechaInicio = DateTime.Now.AddHours(1),
                FechaFin = DateTime.Now,
                LimiteProy = 3,
                Comentarios = false,
                ComentariosObligatorios = false,
                EsAnonima = false,
                EventoId = eventoId
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _votacionService.CrearVotacionAsync(dto)
            );
            Assert.Contains("fecha de inicio debe ser menor", exception.Message);
        }

        [Fact]
        public async Task CrearVotacionAsync_WithInvalidVotationType_ShouldThrowException()
        {
            // Arrange
            var eventoId = Guid.NewGuid().ToString();
            var evento = new Evento("Evento Test", "Descripción", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
            evento.Id = Guid.Parse(eventoId);

            var dto = new CrearVotacionDto
            {
                Nombre = "Votación Test",
                Tipo = "INVALIDA",
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddHours(1),
                LimiteProy = 3,
                Comentarios = false,
                ComentariosObligatorios = false,
                EsAnonima = false,
                EventoId = eventoId
            };

            _mockEventoRepository
                .Setup(x => x.ObtenerPorIdAsync(eventoId))
                .ReturnsAsync(evento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _votacionService.CrearVotacionAsync(dto)
            );
            Assert.Contains("Tipo de votación no válido", exception.Message);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_WithNonexistentVotacion_ShouldReturnNull()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();

            _mockVotacionRepository
                .Setup(x => x.ObtenerAsync(votacionId))
                .ReturnsAsync((Votacion)null);

            // Act
            var result = await _votacionService.ObtenerPorIdAsync(votacionId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task EliminarVotacionAsync_WithNonexistentVotacion_ShouldThrowException()
        {
            // Arrange
            var votacionId = Guid.NewGuid().ToString();

            _mockVotoRepository
                .Setup(x => x.EliminarPorVotacionAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockVotacionRepository
                .Setup(x => x.EliminarAsync(votacionId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _votacionService.EliminarVotacionAsync(votacionId)
            );
            Assert.Contains("No se encontró la votación", exception.Message);
        }

        [Fact]
        public async Task ObtenerTodasAsync_WithNoVotaciones_ShouldReturnEmptyList()
        {
            // Arrange
            _mockVotacionRepository
                .Setup(x => x.ObtenerTodasAsync())
                .ReturnsAsync(new List<Votacion>());

            // Act
            var result = await _votacionService.ObtenerTodasAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerPorEventoAsync_WithInvalidEventId_ShouldReturnEmptyList()
        {
            // Act
            var result = await _votacionService.ObtenerPorEventoAsync("invalid-guid");

            // Assert
            Assert.Empty(result);
        }
    }
}
