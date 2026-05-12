using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Votify.Application.DTOs;
using Votify.Application.Services;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Tests.Services
{
    public class ProyectoServiceTests
    {
        private readonly Mock<IProyectoRepository> _mockProyectoRepository;
        private readonly Mock<IParticipanteRepository> _mockParticipanteRepository;
        private readonly Mock<IParticipanteEventoRepository> _mockParticipanteEventoRepository;
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IEquipoRepository> _mockEquipoRepository;
        private readonly ProyectoService _proyectoService;

        public ProyectoServiceTests()
        {
            _mockProyectoRepository = new Mock<IProyectoRepository>();
            _mockParticipanteRepository = new Mock<IParticipanteRepository>();
            _mockParticipanteEventoRepository = new Mock<IParticipanteEventoRepository>();
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockEquipoRepository = new Mock<IEquipoRepository>();

            _proyectoService = new ProyectoService(
                _mockProyectoRepository.Object,
                _mockParticipanteRepository.Object,
                _mockParticipanteEventoRepository.Object,
                _mockVotacionRepository.Object,
                _mockEquipoRepository.Object);
        }

        [Fact]
        public async Task CrearProyectoAsync_WhenUsuarioEsCompetidor_UsaSuEquipoAutomaticamente()
        {
            // Arrange
            var participanteId = Guid.NewGuid();
            var equipoId = Guid.NewGuid();
            var eventoId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();

            var participante = new Participante("Competidor", "comp@test.com", "hash", equipoId) { Id = participanteId };
            var votacion = new VotacionEstandar("Votación", DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1), 1, false, false, eventoId);
            votacion.Id = votacionId;

            Proyecto? proyectoGuardado = null;

            _mockParticipanteRepository.Setup(x => x.ObtenerPorIdAsync(participanteId)).ReturnsAsync(participante);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(eventoId, participanteId)).ReturnsAsync("COMPETIDOR");
            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId.ToString())).ReturnsAsync(votacion);
            _mockProyectoRepository.Setup(x => x.ObtenerPorVotacionAsync(votacionId.ToString())).ReturnsAsync(new List<Proyecto>());
            _mockProyectoRepository.Setup(x => x.GuardarAsync(It.IsAny<Proyecto>()))
                .Callback<Proyecto>(p => proyectoGuardado = p)
                .Returns(Task.CompletedTask);

            var dto = new ProyectoDto
            {
                Id = string.Empty,
                Nombre = "Proyecto Test",
                Descripcion = "Descripción test",
                Equipo_Id = "otro-equipo-que-debe-ignorarse",
                VotacionId = votacionId,
                ParticipanteId = participanteId
            };

            // Act
            var id = await _proyectoService.CrearProyectoAsync(dto);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(id));
            Assert.NotNull(proyectoGuardado);
            Assert.Equal(equipoId.ToString(), proyectoGuardado!.Equipo_Id);
            _mockEquipoRepository.Verify(x => x.ObtenerPorIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task CrearProyectoAsync_WhenUsuarioEsOrganizador_UsaElEquipoSeleccionadoManual()
        {
            // Arrange
            var participanteId = Guid.NewGuid();
            var eventoId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();
            var equipoSeleccionadoId = Guid.NewGuid();

            var participante = new Participante("Organizador", "orga@test.com", "hash") { Id = participanteId };
            var votacion = new VotacionEstandar("Votación", DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1), 1, false, false, eventoId);
            votacion.Id = votacionId;
            var equipoSeleccionado = new Equipo("Equipo A", equipoSeleccionadoId);

            Proyecto? proyectoGuardado = null;

            _mockParticipanteRepository.Setup(x => x.ObtenerPorIdAsync(participanteId)).ReturnsAsync(participante);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(eventoId, participanteId)).ReturnsAsync("ORGANIZADOR");
            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId.ToString())).ReturnsAsync(votacion);
            _mockEquipoRepository.Setup(x => x.ObtenerPorIdAsync(equipoSeleccionadoId)).ReturnsAsync(equipoSeleccionado);
            _mockProyectoRepository.Setup(x => x.ObtenerPorVotacionAsync(votacionId.ToString())).ReturnsAsync(new List<Proyecto>());
            _mockProyectoRepository.Setup(x => x.GuardarAsync(It.IsAny<Proyecto>()))
                .Callback<Proyecto>(p => proyectoGuardado = p)
                .Returns(Task.CompletedTask);

            var dto = new ProyectoDto
            {
                Id = string.Empty,
                Nombre = "Proyecto Manual",
                Descripcion = "Descripción manual",
                Equipo_Id = equipoSeleccionadoId.ToString(),
                VotacionId = votacionId,
                ParticipanteId = participanteId
            };

            // Act
            var id = await _proyectoService.CrearProyectoAsync(dto);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(id));
            Assert.NotNull(proyectoGuardado);
            Assert.Equal(equipoSeleccionadoId.ToString(), proyectoGuardado!.Equipo_Id);
            _mockEquipoRepository.Verify(x => x.ObtenerPorIdAsync(equipoSeleccionadoId), Times.Once);
        }

        [Fact]
        public async Task CrearProyectoAsync_WhenCompetidorNoTieneEquipo_Throws()
        {
            // Arrange
            var participanteId = Guid.NewGuid();
            var eventoId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();

            var participante = new Participante("Competidor", "comp@test.com", "hash") { Id = participanteId };
            var votacion = new VotacionEstandar("Votación", DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1), 1, false, false, eventoId);
            votacion.Id = votacionId;

            _mockParticipanteRepository.Setup(x => x.ObtenerPorIdAsync(participanteId)).ReturnsAsync(participante);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(eventoId, participanteId)).ReturnsAsync("COMPETIDOR");
            _mockVotacionRepository.Setup(x => x.ObtenerAsync(votacionId.ToString())).ReturnsAsync(votacion);

            var dto = new ProyectoDto
            {
                Id = string.Empty,
                Nombre = "Proyecto Test",
                Descripcion = "Descripción test",
                VotacionId = votacionId,
                ParticipanteId = participanteId
            };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _proyectoService.CrearProyectoAsync(dto));
            _mockProyectoRepository.Verify(x => x.GuardarAsync(It.IsAny<Proyecto>()), Times.Never);
        }
    }
}