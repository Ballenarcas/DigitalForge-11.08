using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Votify.Application.Services;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Tests.Services
{
    public class EquipoServiceTests
    {
        private readonly Mock<IEquipoRepository> _mockEquipoRepository;
        private readonly Mock<IParticipanteRepository> _mockParticipanteRepository;
        private readonly Mock<IParticipanteEventoRepository> _mockParticipanteEventoRepository;
        private readonly EquipoService _equipoService;

        public EquipoServiceTests()
        {
            _mockEquipoRepository = new Mock<IEquipoRepository>();
            _mockParticipanteRepository = new Mock<IParticipanteRepository>();
            _mockParticipanteEventoRepository = new Mock<IParticipanteEventoRepository>();

            _equipoService = new EquipoService(
                _mockEquipoRepository.Object,
                _mockParticipanteRepository.Object,
                _mockParticipanteEventoRepository.Object);
        }

        [Fact]
        public async Task AsignarParticipanteAEquipoAsync_WhenSolicitanteEsMiembroDelEquipo_ShouldAsignarYPromoverACompetidor()
        {
            // Arrange
            var eventoId = Guid.NewGuid();
            var equipoId = Guid.NewGuid();
            var solicitanteId = Guid.NewGuid();
            var participanteId = Guid.NewGuid();

            var equipo = new Equipo("Equipo Test", equipoId);
            var solicitante = new Participante("Solicitante", "solicitante@test.com", "hash", equipoId) { Id = solicitanteId };
            var participante = new Participante("Nuevo", "nuevo@test.com", "hash") { Id = participanteId };

            _mockEquipoRepository.Setup(x => x.ObtenerPorIdAsync(equipoId)).ReturnsAsync(equipo);
            _mockParticipanteRepository.Setup(x => x.ObtenerPorIdAsync(participanteId)).ReturnsAsync(participante);
            _mockParticipanteRepository.Setup(x => x.ObtenerPorIdAsync(solicitanteId)).ReturnsAsync(solicitante);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(eventoId, solicitanteId)).ReturnsAsync("COMPETIDOR");
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(eventoId, participanteId)).ReturnsAsync("PÚBLICO");
            _mockParticipanteEventoRepository.Setup(x => x.ActualizarRolAsync(eventoId, participanteId, "COMPETIDOR")).ReturnsAsync(true);

            // Act
            await _equipoService.AsignarParticipanteAEquipoAsync(solicitanteId, participanteId, equipoId, eventoId);

            // Assert
            _mockParticipanteRepository.Verify(x => x.ActualizarAsync(It.Is<Participante>(p => p.Id == participanteId && p.EquipoId == equipoId)), Times.Once);
            _mockParticipanteEventoRepository.Verify(x => x.ActualizarRolAsync(eventoId, participanteId, "COMPETIDOR"), Times.Once);
            _mockParticipanteEventoRepository.Verify(x => x.GuardarAsync(It.IsAny<ParticipanteEvento>()), Times.Never);
        }

        [Fact]
        public async Task AsignarParticipanteAEquipoAsync_WhenSolicitanteEsOrganizador_ShouldPermitirAsignacion()
        {
            // Arrange
            var eventoId = Guid.NewGuid();
            var equipoId = Guid.NewGuid();
            var solicitanteId = Guid.NewGuid();
            var participanteId = Guid.NewGuid();

            var equipo = new Equipo("Equipo Test", equipoId);
            var solicitante = new Participante("Organizador", "organizador@test.com", "hash") { Id = solicitanteId };
            var participante = new Participante("Nuevo", "nuevo@test.com", "hash") { Id = participanteId };

            _mockEquipoRepository.Setup(x => x.ObtenerPorIdAsync(equipoId)).ReturnsAsync(equipo);
            _mockParticipanteRepository.Setup(x => x.ObtenerPorIdAsync(participanteId)).ReturnsAsync(participante);
            _mockParticipanteRepository.Setup(x => x.ObtenerPorIdAsync(solicitanteId)).ReturnsAsync(solicitante);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(eventoId, solicitanteId)).ReturnsAsync("ORGANIZADOR");
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(eventoId, participanteId)).ReturnsAsync((string?)null);

            // Act
            await _equipoService.AsignarParticipanteAEquipoAsync(solicitanteId, participanteId, equipoId, eventoId);

            // Assert
            _mockParticipanteRepository.Verify(x => x.ActualizarAsync(It.Is<Participante>(p => p.Id == participanteId && p.EquipoId == equipoId)), Times.Once);
            _mockParticipanteEventoRepository.Verify(x => x.GuardarAsync(It.Is<ParticipanteEvento>(pe => pe.ParticipanteId == participanteId && pe.EventoId == eventoId && pe.Rol == "COMPETIDOR")), Times.Once);
        }

        [Fact]
        public async Task AsignarParticipanteAEquipoAsync_WhenSolicitanteNoTienePermisos_ShouldLanzarUnauthorized()
        {
            // Arrange
            var eventoId = Guid.NewGuid();
            var equipoId = Guid.NewGuid();
            var solicitanteId = Guid.NewGuid();
            var participanteId = Guid.NewGuid();

            var equipo = new Equipo("Equipo Test", equipoId);
            var solicitante = new Participante("Invitado", "invitado@test.com", "hash") { Id = solicitanteId };
            var participante = new Participante("Nuevo", "nuevo@test.com", "hash") { Id = participanteId };

            _mockEquipoRepository.Setup(x => x.ObtenerPorIdAsync(equipoId)).ReturnsAsync(equipo);
            _mockParticipanteRepository.Setup(x => x.ObtenerPorIdAsync(participanteId)).ReturnsAsync(participante);
            _mockParticipanteRepository.Setup(x => x.ObtenerPorIdAsync(solicitanteId)).ReturnsAsync(solicitante);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(eventoId, solicitanteId)).ReturnsAsync("PÚBLICO");

            // Act + Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _equipoService.AsignarParticipanteAEquipoAsync(solicitanteId, participanteId, equipoId, eventoId));

            _mockParticipanteRepository.Verify(x => x.ActualizarAsync(It.IsAny<Participante>()), Times.Never);
            _mockParticipanteEventoRepository.Verify(x => x.GuardarAsync(It.IsAny<ParticipanteEvento>()), Times.Never);
            _mockParticipanteEventoRepository.Verify(x => x.ActualizarRolAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }
    }
}