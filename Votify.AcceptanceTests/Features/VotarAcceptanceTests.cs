using Votify.AcceptanceTests.Helpers;
using Votify.Application.DTOs;
using Votify.Application.Services;
using Votify.Infrastructure.Repositories;
using Xunit;

namespace Votify.AcceptanceTests.Features
{
    /// <summary>
    /// Pruebas de aceptación para la funcionalidad de votar
    /// 
    /// Escenarios de aceptación:
    /// - Votante estándar puede votar en una votación
    /// - Votante anónimo puede votar en una votación
    /// - No permite votar si se alcanzó el límite de votos
    /// - No permite votar en una votación inexistente
    /// - Registra correctamente múltiples votos hasta el límite
    /// </summary>
    public class VotarAcceptanceTests : AcceptanceTestBase
    {
        private VotoService _votoService = null!;
        private VotacionRepository _votacionRepository = null!;
        private ProyectoRepository _proyectoRepository = null!;
        private VotoRepository _votoRepository = null!;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // Inicializar repositorios y el servicio
            _votacionRepository = new VotacionRepository(DbContext);
            _proyectoRepository = new ProyectoRepository(DbContext);
            _votoRepository = new VotoRepository(DbContext);
            _votoService = new VotoService(_votoRepository, _votacionRepository);
        }

        #region Casos de Éxito

        [Fact]
        public async Task Votar_UsuarioEstandarEnVotacionEstandar_DebeRegistrarVotoExitosamente()
        {
            // Arrange: Crear votación y proyectos
            var votacion = TestDataFactory.CrearVotacionEstandar(nombre: "Votación Estándar", limiteProyectos: 2);
            var proyecto = TestDataFactory.CrearProyecto(nombre: "Proyecto A");
            var votanteId = Guid.NewGuid().ToString();

            DbContext.Votaciones.Add(votacion);
            DbContext.Proyectos.Add(proyecto);
            await DbContext.SaveChangesAsync();

            var votarDto = new VotarDto
            {
                VotacionId = votacion.Id.ToString(),
                ProyectoId = proyecto.Id.ToString(),
                VotanteId = votanteId
            };

            // Act
            await _votoService.VotarAsync(votarDto);

            // Assert
            var votosGuardados = await _votoRepository.ObtenerPorProyectoAsync(proyecto.Id.ToString());
            Assert.Single(votosGuardados);
            Assert.Equal(votanteId, votosGuardados.First().VotanteId);
        }

        [Fact]
        public async Task Votar_UsuarioAnonimoEnVotacionAnonima_DebeRegistrarVotoExitosamente()
        {
            // Arrange: Crear votación anónima y proyecto
            var votacion = TestDataFactory.CrearVotacionAnonima(nombre: "Votación Anónima", limiteProyectos: 3);
            var proyecto = TestDataFactory.CrearProyecto(nombre: "Proyecto B");

            DbContext.Votaciones.Add(votacion);
            DbContext.Proyectos.Add(proyecto);
            await DbContext.SaveChangesAsync();

            var votarDto = new VotarDto
            {
                VotacionId = votacion.Id.ToString(),
                ProyectoId = proyecto.Id.ToString(),
                VotanteId = null // Voto anónimo
            };

            // Act
            await _votoService.VotarAsync(votarDto);

            // Assert
            var votosGuardados = await _votoRepository.ObtenerPorProyectoAsync(proyecto.Id.ToString());
            Assert.Single(votosGuardados);
            Assert.Null(votosGuardados.First().VotanteId);
        }

        [Fact]
        public async Task Votar_UsuarioPuedeMúltiplesVotosHastaLimite_DebeRegistrarTodos()
        {
            // Arrange: Crear votación con límite de 2 votos
            var votacion = TestDataFactory.CrearVotacionEstandar(nombre: "Votación Multi", limiteProyectos: 2);
            var proyecto1 = TestDataFactory.CrearProyecto(nombre: "Proyecto 1");
            var proyecto2 = TestDataFactory.CrearProyecto(nombre: "Proyecto 2");
            var votanteId = Guid.NewGuid().ToString();

            DbContext.Votaciones.Add(votacion);
            DbContext.Proyectos.AddRange(proyecto1, proyecto2);
            await DbContext.SaveChangesAsync();

            // Act: Votar dos veces (dentro del límite)
            await _votoService.VotarAsync(new VotarDto
            {
                VotacionId = votacion.Id.ToString(),
                ProyectoId = proyecto1.Id.ToString(),
                VotanteId = votanteId
            });

            await _votoService.VotarAsync(new VotarDto
            {
                VotacionId = votacion.Id.ToString(),
                ProyectoId = proyecto2.Id.ToString(),
                VotanteId = votanteId
            });

            // Assert
            var votosUsuario = await _votoRepository.ContarVotosPorUsuarioYVotacionAsync(votacion.Id.ToString(), votanteId);
            Assert.Equal(2, votosUsuario);
        }

        [Fact]
        public async Task Votar_ConsultarResultadosPorVotacion_DebeRetornarVotosOrdenados()
        {
            // Arrange
            var votacion = TestDataFactory.CrearVotacionEstandar(nombre: "Votación Resultados", limiteProyectos: 3);
            var proyecto1 = TestDataFactory.CrearProyecto(nombre: "Proyecto Popular");
            var proyecto2 = TestDataFactory.CrearProyecto(nombre: "Proyecto Menos Popular");

            DbContext.Votaciones.Add(votacion);
            DbContext.Proyectos.AddRange(proyecto1, proyecto2);
            await DbContext.SaveChangesAsync();

            // Act: Registrar 3 votos para proyecto1 y 1 voto para proyecto2
            for (int i = 0; i < 3; i++)
            {
                await _votoService.VotarAsync(new VotarDto
                {
                    VotacionId = votacion.Id.ToString(),
                    ProyectoId = proyecto1.Id.ToString(),
                    VotanteId = Guid.NewGuid().ToString()
                });
            }

            await _votoService.VotarAsync(new VotarDto
            {
                VotacionId = votacion.Id.ToString(),
                ProyectoId = proyecto2.Id.ToString(),
                VotanteId = Guid.NewGuid().ToString()
            });

            // Assert
            var resultados = await _votoRepository.ObtenerVotosPorVotacionAsync(votacion.Id.ToString());
            Assert.Equal(2, resultados.Count);
            Assert.Equal(3, resultados[0].Votos); // Primer resultado tiene 3 votos
            Assert.Equal(1, resultados[1].Votos); // Segundo resultado tiene 1 voto
        }

        #endregion

        #region Casos de Error

        [Fact]
        public async Task Votar_CuandoSeAlcanzaLimite_LanzaInvalidOperationException()
        {
            // Arrange: Votación con límite de 1
            var votacion = TestDataFactory.CrearVotacionEstandar(nombre: "Límite Bajo", limiteProyectos: 1);
            var proyecto1 = TestDataFactory.CrearProyecto(nombre: "Proyecto 1");
            var proyecto2 = TestDataFactory.CrearProyecto(nombre: "Proyecto 2");
            var votanteId = Guid.NewGuid().ToString();

            DbContext.Votaciones.Add(votacion);
            DbContext.Proyectos.AddRange(proyecto1, proyecto2);
            await DbContext.SaveChangesAsync();

            // Act: Primer voto OK
            await _votoService.VotarAsync(new VotarDto
            {
                VotacionId = votacion.Id.ToString(),
                ProyectoId = proyecto1.Id.ToString(),
                VotanteId = votanteId
            });

            // Assert: Segundo voto falla
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _votoService.VotarAsync(new VotarDto
                {
                    VotacionId = votacion.Id.ToString(),
                    ProyectoId = proyecto2.Id.ToString(),
                    VotanteId = votanteId
                })
            );

            Assert.Contains("No puedes votar", ex.Message);
            Assert.Contains("límite", ex.Message);
        }

        [Fact]
        public async Task Votar_ConVotacionInexistente_LanzaArgumentException()
        {
            // Arrange
            var votacionIdInexistente = Guid.NewGuid().ToString();
            var proyecto = TestDataFactory.CrearProyecto();

            DbContext.Proyectos.Add(proyecto);
            await DbContext.SaveChangesAsync();

            var votarDto = new VotarDto
            {
                VotacionId = votacionIdInexistente,
                ProyectoId = proyecto.Id.ToString(),
                VotanteId = Guid.NewGuid().ToString()
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _votoService.VotarAsync(votarDto)
            );

            Assert.Contains("no existe", ex.Message);
        }

        [Fact]
        public async Task Votar_VariosUsuariosAnonimosPuedenVotar_SinConflictos()
        {
            // Arrange: Votación anónima donde múltiples usuarios anónimos pueden votar
            var votacion = TestDataFactory.CrearVotacionAnonima(nombre: "Anónima Multi", limiteProyectos: 1);
            var proyecto = TestDataFactory.CrearProyecto();

            DbContext.Votaciones.Add(votacion);
            DbContext.Proyectos.Add(proyecto);
            await DbContext.SaveChangesAsync();

            // Act: Múltiples usuarios anónimos votan
            for (int i = 0; i < 3; i++)
            {
                await _votoService.VotarAsync(new VotarDto
                {
                    VotacionId = votacion.Id.ToString(),
                    ProyectoId = proyecto.Id.ToString(),
                    VotanteId = null // Anónimos
                });
            }

            // Assert
            var votosProyecto = await _votoRepository.ObtenerPorProyectoAsync(proyecto.Id.ToString());
            Assert.Equal(3, votosProyecto.Count);
            Assert.All(votosProyecto, v => Assert.Null(v.VotanteId));
        }

        #endregion
    }
}
