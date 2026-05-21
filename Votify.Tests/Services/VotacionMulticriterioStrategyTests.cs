using Moq;
using Xunit;
using Votify.Application.DTOs;
using Votify.Application.Services.Estrategia;
using Votify.Tests.Builders;
using Votify.Domain.Entities;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Votify.Tests.Services
{
    public class VotacionMulticriterioStrategyTests
    {
        private readonly Mock<ICriterioRepository> _mockCriterioRepository;
        private readonly Mock<IValoracionCriterioRepository> _mockValoracionCriterioRepository;
        private readonly Mock<IComentarioRepository> _mockComentarioRepository;
        private readonly Mock<IParticipanteEventoRepository> _mockParticipanteEventoRepository;
        private readonly Mock<IProyectoRepository> _mockProyectoRepository;
        private readonly Mock<IEquipoRepository> _mockEquipoRepository;
        private readonly VotacionMulticriterioStrategy _strategy;

        public VotacionMulticriterioStrategyTests()
        {
            _mockCriterioRepository = new Mock<ICriterioRepository>();
            _mockValoracionCriterioRepository = new Mock<IValoracionCriterioRepository>();
            _mockComentarioRepository = new Mock<IComentarioRepository>();
            _mockParticipanteEventoRepository = new Mock<IParticipanteEventoRepository>();
            _mockProyectoRepository = new Mock<IProyectoRepository>();
            _mockEquipoRepository = new Mock<IEquipoRepository>();

            _strategy = new VotacionMulticriterioStrategy(
                _mockCriterioRepository.Object,
                _mockValoracionCriterioRepository.Object,
                _mockComentarioRepository.Object,
                _mockParticipanteEventoRepository.Object,
                _mockProyectoRepository.Object,
                _mockEquipoRepository.Object
            );
        }

        [Fact]
        public void Tipo_ShouldReturnMulticriterio()
        {
            Assert.Equal("MULTICRITERIO", _strategy.Tipo);
        }

        [Fact]
        public async Task ProcesarVotoMulticriterioAsync_WithoutComment_ShouldThrowException()
        {
            var votacion = new VotacionMulticriterio("Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), 3, false, false, Guid.NewGuid());
            var dto = new VotoMulticriterioDto
            {
                VotacionId = Guid.NewGuid().ToString(),
                ProyectoId = Guid.NewGuid().ToString(),
                VotanteId = Guid.NewGuid().ToString(),
                Comentario = "   "
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _strategy.ProcesarVotoMulticriterioAsync(votacion, dto));
            Assert.Contains("comentario es obligatorio", ex.Message);
        }

        [Fact]
        public async Task ProcesarVotoMulticriterioAsync_AlreadyVoted_ShouldThrowException()
        {
            var votacion = new VotacionMulticriterio("Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), 3, false, false, Guid.NewGuid());
            var dto = new VotoMulticriterioDto
            {
                VotacionId = Guid.NewGuid().ToString(),
                ProyectoId = Guid.NewGuid().ToString(),
                VotanteId = Guid.NewGuid().ToString(),
                Comentario = "Buen proyecto"
            };

            _mockValoracionCriterioRepository.Setup(x => x.HaValoradoProyectoAsync(dto.ProyectoId, dto.VotanteId)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _strategy.ProcesarVotoMulticriterioAsync(votacion, dto));
            Assert.Contains("voto ya ha sido emitido", ex.Message);
        }

        [Fact]
        public async Task ProcesarVotoMulticriterioAsync_WithInvalidRole_ShouldThrowException()
        {
            var eventoId = Guid.NewGuid();
            var votacion = new VotacionMulticriterio("Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), 3, false, false, eventoId);
            var votanteId = Guid.NewGuid().ToString();
            var dto = new VotoMulticriterioDto
            {
                VotacionId = Guid.NewGuid().ToString(),
                ProyectoId = Guid.NewGuid().ToString(),
                VotanteId = votanteId,
                Comentario = "Buen proyecto"
            };

            _mockValoracionCriterioRepository.Setup(x => x.HaValoradoProyectoAsync(dto.ProyectoId, dto.VotanteId)).ReturnsAsync(false);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(eventoId, Guid.Parse(votanteId))).ReturnsAsync("COMPETIDOR");

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _strategy.ProcesarVotoMulticriterioAsync(votacion, dto));
            Assert.Contains("Solo el rol Jurado o Público", ex.Message);
        }

        [Fact]
        public async Task ProcesarVotoMulticriterioAsync_WithValidData_ShouldSaveValoraciones()
        {
            var eventoId = Guid.NewGuid();
            var votacionId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();
            var votanteId = Guid.NewGuid().ToString();
            var criterioId = Guid.NewGuid();

            var votacion = new VotacionMulticriterio("Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), 3, false, false, eventoId);
            var dto = new VotoMulticriterioDto
            {
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = votanteId,
                Comentario = "Excelente",
                Valoraciones = new List<ValoracionCriterioDto>
                {
                    new ValoracionCriterioDto { CriterioId = criterioId.ToString(), Valoracion = 5 }
                }
            };

            _mockValoracionCriterioRepository.Setup(x => x.HaValoradoProyectoAsync(proyectoId, votanteId)).ReturnsAsync(false);
            _mockParticipanteEventoRepository.Setup(x => x.ObtenerRolAsync(eventoId, Guid.Parse(votanteId))).ReturnsAsync("Jurado");
            _mockCriterioRepository.Setup(x => x.ObtenerPorVotacionAsync(votacionId))
                .ReturnsAsync(new List<Criterio> { new Criterio { Id = criterioId, Nombre = "Calidad", Peso = 100 } });

            await _strategy.ProcesarVotoMulticriterioAsync(votacion, dto);

            _mockValoracionCriterioRepository.Verify(x => x.GuardarAsync(proyectoId, votanteId, It.Is<List<ValoracionCriterio>>(v => v.Count == 1 && v[0].Valoracion == 5)), Times.Once);
            _mockComentarioRepository.Verify(x => x.GuardarAsync(proyectoId, "Excelente", Guid.Parse(votanteId)), Times.Once);
        }

        [Fact]
        public async Task CalcularResultadosMulticriterioAsync_WithData_ShouldReturnDetailedResults()
        {
            var votacionId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();
            var criterioId = Guid.NewGuid();
            var equipoId = Guid.NewGuid();

            _mockValoracionCriterioRepository.Setup(x => x.ObtenerResultadosPonderadosAsync(votacionId))
                .ReturnsAsync(new List<(string ProyectoId, double Puntaje, int Evaluaciones)> { (proyectoId, 85.5, 3) });
            _mockValoracionCriterioRepository.Setup(x => x.ObtenerDetallesPorCriterioAsync(votacionId))
                .ReturnsAsync(new List<(string ProyectoId, Guid CriterioId, double PromedioValoracion, int NumEvaluaciones)> { (proyectoId, criterioId, 4.5, 3) });
            _mockCriterioRepository.Setup(x => x.ObtenerPorVotacionAsync(votacionId))
                .ReturnsAsync(new List<Criterio> { new Criterio { Id = criterioId, Nombre = "Calidad", Peso = 100 } });
            _mockProyectoRepository.Setup(x => x.ObtenerPorVotacionAsync(votacionId))
                .ReturnsAsync(new List<Proyecto> { new Proyecto("Proyecto A", "Desc", equipoId.ToString(), Guid.Parse(votacionId), id: proyectoId) });
            _mockEquipoRepository.Setup(x => x.ObtenerTodosAsync())
                .ReturnsAsync(new List<Equipo> { new Equipo("Equipo A") { Id = equipoId } });

            var result = await _strategy.CalcularResultadosMulticriterioAsync(votacionId);

            Assert.Single(result);
            Assert.Equal(proyectoId, result[0].Id);
            Assert.Equal(85.5, result[0].PuntajeFinal);
            Assert.Single(result[0].DetallesCriterios);
            Assert.Equal("Calidad", result[0].DetallesCriterios[0].CriterioNombre);
        }
    }
}
