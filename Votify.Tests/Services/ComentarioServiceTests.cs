using Moq;
using Xunit;
using Votify.Application.DTOs;
using Votify.Application.Services;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using System;

namespace Votify.Tests.Services
{
    public class ComentarioServiceTests
    {
        private readonly Mock<IComentarioRepository> _mockComentarioRepository;
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly ComentarioService _comentarioService;

        public ComentarioServiceTests()
        {
            _mockComentarioRepository = new Mock<IComentarioRepository>();
            _mockVotacionRepository = new Mock<IVotacionRepository>();

            _comentarioService = new ComentarioService(
                _mockComentarioRepository.Object,
                _mockVotacionRepository.Object
            );
        }

        #region AgregarComentarioAsync Tests

        [Fact]
        public async Task AgregarComentarioAsync_WithValidData_ShouldSaveComment()
        {
            // Arrange
            var proyectoId = Guid.NewGuid().ToString();
            var autorId = Guid.NewGuid();
            var texto = "Este es un comentario válido";

            _mockComentarioRepository
                .Setup(x => x.HaComentadoProyectoAsync(proyectoId, autorId))
                .ReturnsAsync(false);

            // Act
            await _comentarioService.AgregarComentarioAsync(proyectoId, texto, autorId);

            // Assert
            _mockComentarioRepository.Verify(
                x => x.GuardarAsync(proyectoId, texto, autorId),
                Times.Once
            );
        }

        [Fact]
        public async Task AgregarComentarioAsync_WithEmptyText_ShouldThrowException()
        {
            // Arrange
            var proyectoId = Guid.NewGuid().ToString();
            var autorId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _comentarioService.AgregarComentarioAsync(proyectoId, "", autorId)
            );
        }

        [Fact]
        public async Task AgregarComentarioAsync_WithWhitespaceText_ShouldThrowException()
        {
            // Arrange
            var proyectoId = Guid.NewGuid().ToString();
            var autorId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _comentarioService.AgregarComentarioAsync(proyectoId, "   ", autorId)
            );
        }

        [Fact]
        public async Task AgregarComentarioAsync_WithDuplicateComment_ShouldThrowException()
        {
            // Arrange
            var proyectoId = Guid.NewGuid().ToString();
            var autorId = Guid.NewGuid();
            var texto = "Comentario duplicado";

            _mockComentarioRepository
                .Setup(x => x.HaComentadoProyectoAsync(proyectoId, autorId))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _comentarioService.AgregarComentarioAsync(proyectoId, texto, autorId)
            );
            Assert.Contains("Solo puedes dejar un comentario por proyecto", exception.Message);
        }

        [Fact]
        public async Task AgregarComentarioAsync_WithAnonymousUser_ShouldSaveComment()
        {
            // Arrange
            var proyectoId = Guid.NewGuid().ToString();
            var texto = "Comentario anónimo";

            // Act
            await _comentarioService.AgregarComentarioAsync(proyectoId, texto, null);

            // Assert
            _mockComentarioRepository.Verify(
                x => x.GuardarAsync(proyectoId, texto, null),
                Times.Once
            );
        }

        #endregion

        #region ObtenerComentariosAsync Tests

        [Fact]
        public async Task ObtenerComentariosAsync_ShouldReturnCommentsWithAuthorInfo()
        {
            // Arrange
            var proyectoId = Guid.NewGuid().ToString();
            var autorId = (Guid?)Guid.NewGuid();

            var comentarioEntity = new Comentario
            {
                Texto = "Comentario de prueba",
                AutorId = autorId,
                AutorNombre = "Usuario Test",
                FechaCreacion = DateTime.Now
            };

            _mockComentarioRepository
                .Setup(x => x.ObtenerAsync(proyectoId))
                .ReturnsAsync(new List<Comentario> { comentarioEntity });

            // Act
            var result = await _comentarioService.ObtenerComentariosAsync(proyectoId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Comentario de prueba", result[0].Texto);
            Assert.Equal(autorId, result[0].AutorId);
            Assert.Equal("Usuario Test", result[0].AutorNombre);
            Assert.False(result[0].EsAnonimo);
        }

        [Fact]
        public async Task ObtenerComentariosAsync_WithAnonymousVotation_ShouldHideAuthorInfo()
        {
            // Arrange
            var proyectoId = Guid.NewGuid().ToString();
            var votacionId = Guid.NewGuid().ToString();
            var autorId = Guid.NewGuid();

            var comentarioEntity = new Comentario
            {
                Texto = "Comentario en votación anónima",
                AutorId = autorId,
                AutorNombre = "Usuario Test",
                FechaCreacion = DateTime.Now
            };

            var votacion = new VotacionEstandar(
                "Votación Anónima",
                DateTime.Now.AddHours(-1),
                DateTime.Now.AddHours(1),
                3,
                true,
                false,
                Guid.NewGuid(),
                true
            );

            _mockComentarioRepository
                .Setup(x => x.ObtenerAsync(proyectoId))
                .ReturnsAsync(new List<Comentario> { comentarioEntity });

            _mockVotacionRepository
                .Setup(x => x.ObtenerAsync(votacionId))
                .ReturnsAsync(votacion);

            // Act
            var result = await _comentarioService.ObtenerComentariosAsync(proyectoId, votacionId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Comentario en votación anónima", result[0].Texto);
            Assert.Null(result[0].AutorId);
            Assert.Null(result[0].AutorNombre);
            Assert.True(result[0].EsAnonimo);
        }

        [Fact]
        public async Task ObtenerComentariosAsync_WithAnonymousComment_ShouldMarkAsAnonymous()
        {
            // Arrange
            var proyectoId = Guid.NewGuid().ToString();

            var comentarioEntity = new Comentario
            {
                Texto = "Comentario anónimo",
                AutorId = null,
                AutorNombre = null,
                FechaCreacion = DateTime.Now
            };

            _mockComentarioRepository
                .Setup(x => x.ObtenerAsync(proyectoId))
                .ReturnsAsync(new List<Comentario> { comentarioEntity });

            // Act
            var result = await _comentarioService.ObtenerComentariosAsync(proyectoId);

            // Assert
            Assert.Single(result);
            Assert.True(result[0].EsAnonimo);
            Assert.Null(result[0].AutorId);
            Assert.Null(result[0].AutorNombre);
        }

        [Fact]
        public async Task ObtenerComentariosAsync_WithNoComments_ShouldReturnEmptyList()
        {
            // Arrange
            var proyectoId = Guid.NewGuid().ToString();

            _mockComentarioRepository
                .Setup(x => x.ObtenerAsync(proyectoId))
                .ReturnsAsync(new List<Comentario>());

            // Act
            var result = await _comentarioService.ObtenerComentariosAsync(proyectoId);

            // Assert
            Assert.Empty(result);
        }

        #endregion
    }
}
