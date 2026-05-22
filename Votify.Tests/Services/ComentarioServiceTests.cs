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
        private readonly Mock<IProyectoRepository> _mockProyectoRepository;
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IParticipanteRepository> _mockParticipanteRepository;
        private readonly Mock<IParticipanteEventoRepository> _mockParticipanteEventoRepository;
        private readonly ComentarioService _comentarioService;

        private readonly Guid _usuarioId = Guid.NewGuid();
        private readonly Guid _equipoId = Guid.NewGuid();
        private readonly Guid _eventoId = Guid.NewGuid();
        private readonly Guid _votacionId = Guid.NewGuid();
        private readonly string _proyectoId = Guid.NewGuid().ToString();

        public ComentarioServiceTests()
        {
            _mockComentarioRepository = new Mock<IComentarioRepository>();
            _mockProyectoRepository = new Mock<IProyectoRepository>();
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockParticipanteRepository = new Mock<IParticipanteRepository>();
            _mockParticipanteEventoRepository = new Mock<IParticipanteEventoRepository>();

            ConfigurarAccesoMiembroEquipo();

            _comentarioService = new ComentarioService(
                _mockComentarioRepository.Object,
                _mockProyectoRepository.Object,
                _mockVotacionRepository.Object
                , _mockParticipanteRepository.Object,
                _mockParticipanteEventoRepository.Object
            );
        }

        private void ConfigurarAccesoMiembroEquipo()
        {
            var proyecto = new Proyecto("Proyecto", "Descripcion", _equipoId.ToString(), _votacionId, null, _proyectoId);
            var votacion = new VotacionEstandar(
                "Votacion",
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1),
                3,
                false,
                false,
                _eventoId,
                false);

            _mockProyectoRepository
                .Setup(x => x.ObtenerAsync(_proyectoId))
                .ReturnsAsync(proyecto);

            _mockVotacionRepository
                .Setup(x => x.ObtenerAsync(_votacionId.ToString()))
                .ReturnsAsync(votacion);

            _mockParticipanteRepository
                .Setup(x => x.ObtenerPorIdAsync(_usuarioId))
                .ReturnsAsync(new Participante("Usuario Test", "test@example.com", "hash", _equipoId)
                {
                    Id = _usuarioId
                });

            _mockParticipanteEventoRepository
                .Setup(x => x.ObtenerRolAsync(_eventoId, _usuarioId))
                .ReturnsAsync((string?)null);
        }

        #region AgregarComentarioAsync Tests

        [Fact]
        public async Task AgregarComentarioAsync_WithValidData_ShouldSaveComment()
        {
            // Arrange
            var proyectoId = _proyectoId;
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
            var proyectoId = _proyectoId;
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
            var proyectoId = _proyectoId;
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
            var proyectoId = _proyectoId;
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
            var proyectoId = _proyectoId;
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
            var proyectoId = _proyectoId;
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
            var result = await _comentarioService.ObtenerComentariosAsync(proyectoId, _usuarioId);

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
            var proyectoId = _proyectoId;
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
                _eventoId,
                true
            );

            _mockComentarioRepository
                .Setup(x => x.ObtenerAsync(proyectoId))
                .ReturnsAsync(new List<Comentario> { comentarioEntity });

            _mockVotacionRepository
                .Setup(x => x.ObtenerAsync(_votacionId.ToString()))
                .ReturnsAsync(votacion);

            // Act
            var result = await _comentarioService.ObtenerComentariosAsync(proyectoId, _usuarioId, _votacionId.ToString());

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
            var proyectoId = _proyectoId;

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
            var result = await _comentarioService.ObtenerComentariosAsync(proyectoId, _usuarioId);

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
            var proyectoId = _proyectoId;

            _mockComentarioRepository
                .Setup(x => x.ObtenerAsync(proyectoId))
                .ReturnsAsync(new List<Comentario>());

            // Act
            var result = await _comentarioService.ObtenerComentariosAsync(proyectoId, _usuarioId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerComentariosAsync_WhenUserIsNotMemberOrOrganizer_ShouldThrowUnauthorized()
        {
            var proyectoId = _proyectoId;
            var otroUsuarioId = Guid.NewGuid();

            _mockParticipanteRepository
                .Setup(x => x.ObtenerPorIdAsync(otroUsuarioId))
                .ReturnsAsync(new Participante("Otro", "otro@example.com", "hash", Guid.NewGuid())
                {
                    Id = otroUsuarioId
                });

            _mockParticipanteEventoRepository
                .Setup(x => x.ObtenerRolAsync(_eventoId, otroUsuarioId))
                .ReturnsAsync((string?)null);

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _comentarioService.ObtenerComentariosAsync(proyectoId, otroUsuarioId)
            );

            Assert.Contains("No tienes permisos", exception.Message);
        }

        #endregion
    }
}
