using Xunit;
using Votify.Domain.Entities;
using System;

namespace Votify.Tests.Domain
{
    public class VotacionTests
    {
        #region ValidarVoto Tests

        [Fact]
        public void ValidarVoto_WithinTimeRange_ShouldNotThrow()
        {
            // Arrange
            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(1),
                3,
                false,
                false,
                Guid.NewGuid()
            );

            // Act & Assert - Should not throw
            votacion.ValidarVoto();
        }

        [Fact]
        public void ValidarVoto_BeforeStartDate_ShouldThrowException()
        {
            // Arrange
            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow.AddHours(1),
                DateTime.UtcNow.AddHours(2),
                3,
                false,
                false,
                Guid.NewGuid()
            );

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => votacion.ValidarVoto());
            Assert.Contains("La votación no está dentro del período permitido", exception.Message);
        }

        [Fact]
        public void ValidarVoto_AfterEndDate_ShouldThrowException()
        {
            // Arrange
            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow.AddHours(-2),
                DateTime.UtcNow.AddHours(-1),
                3,
                false,
                false,
                Guid.NewGuid()
            );

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => votacion.ValidarVoto());
            Assert.Contains("La votación no está dentro del período permitido", exception.Message);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldSetAllProperties()
        {
            // Arrange
            var nombre = "Votación Test";
            var inicio = DateTime.UtcNow;
            var fin = DateTime.UtcNow.AddHours(1);
            var limite = 3;
            var comentarios = true;
            var comentariosObligatorios = true;
            var esAnonima = false;
            var eventoId = Guid.NewGuid();

            // Act
            var votacion = new VotacionEstandar(
                nombre, inicio, fin, limite, comentarios, comentariosObligatorios, eventoId, esAnonima
            );

            // Assert
            Assert.Equal(nombre, votacion.Nombre);
            Assert.Equal(inicio, votacion.FechaInicio);
            Assert.Equal(fin, votacion.FechaFin);
            Assert.Equal(limite, votacion.LimiteProy);
            Assert.Equal(comentarios, votacion.Comentarios);
            Assert.Equal(comentariosObligatorios, votacion.ComentariosObligatorios);
            Assert.Equal("ESTANDAR", votacion.Tipo);
            Assert.Equal(esAnonima, votacion.EsAnonima);
            Assert.Equal(eventoId, votacion.EventoId);
            Assert.NotEqual(Guid.Empty, votacion.Id);
        }

        #endregion

        #region Estado Tests

        [Fact]
        public void NewVotacion_ShouldHaveOpenStatus()
        {
            // Arrange & Act
            var votacion = new VotacionEstandar(
                "Votación Test",
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                3,
                false,
                false,
                Guid.NewGuid()
            );

            // Assert
            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
        }

        #endregion
    }

    public class VotoTests
    {
        [Fact]
        public void VotoEstandar_ShouldHaveVotanteId()
        {
            // Arrange
            var votanteId = Guid.NewGuid().ToString();
            var proyectoId = Guid.NewGuid().ToString();
            var votacionId = Guid.NewGuid().ToString();

            // Act
            var voto = new VotoEstandar(proyectoId, votanteId, votacionId);

            // Assert
            Assert.Equal(votanteId, voto.VotanteId);
            Assert.Equal(proyectoId, voto.ProyectoId);
            Assert.Equal(votacionId, voto.VotacionId);
            Assert.Equal("ESTANDAR", voto.Tipo());
        }

        [Fact]
        public void VotoAnonimo_ShouldNotHaveVotanteId()
        {
            // Arrange
            var proyectoId = Guid.NewGuid().ToString();
            var votacionId = Guid.NewGuid().ToString();

            // Act
            var voto = new VotoAnonimo(proyectoId, votacionId);

            // Assert
            Assert.Null(voto.VotanteId);
            Assert.Equal(proyectoId, voto.ProyectoId);
            Assert.Equal(votacionId, voto.VotacionId);
            Assert.Equal("ANONIMO", voto.Tipo());
        }
    }

    public class ComentarioTests
    {
        [Fact]
        public void Comentario_ShouldHaveCorrectProperties()
        {
            // Arrange
            var texto = "Comentario de prueba";
            var autorId = Guid.NewGuid();
            var autorNombre = "Usuario Test";
            var fecha = DateTime.Now;

            // Act
            var comentario = new Comentario
            {
                Texto = texto,
                AutorId = autorId,
                AutorNombre = autorNombre,
                FechaCreacion = fecha
            };

            // Assert
            Assert.Equal(texto, comentario.Texto);
            Assert.Equal(autorId, comentario.AutorId);
            Assert.Equal(autorNombre, comentario.AutorNombre);
            Assert.Equal(fecha, comentario.FechaCreacion);
        }

        [Fact]
        public void Comentario_ShouldSupportAnonymous()
        {
            // Arrange
            var texto = "Comentario anónimo";

            // Act
            var comentario = new Comentario
            {
                Texto = texto,
                AutorId = null,
                AutorNombre = null,
                FechaCreacion = DateTime.Now
            };

            // Assert
            Assert.Equal(texto, comentario.Texto);
            Assert.Null(comentario.AutorId);
            Assert.Null(comentario.AutorNombre);
        }
    }

    public class EventoTests
    {
        [Fact]
        public void Constructor_ShouldSetAllProperties()
        {
            // Arrange
            var nombre = "Evento Test";
            var descripcion = "Descripción del evento";
            var inicio = DateTime.Now;
            var fin = DateTime.Now.AddDays(1);

            // Act
            var evento = new Evento(nombre, descripcion, inicio, fin);

            // Assert
            Assert.Equal(nombre, evento.Nombre);
            Assert.Equal(descripcion, evento.Descripcion);
            Assert.Equal(inicio, evento.FechaInicio);
            Assert.Equal(fin, evento.FechaFin);
            Assert.NotEqual(Guid.Empty, evento.Id);
        }

        [Fact]
        public void Evento_WithPastDate_ShouldBeCreatedSuccessfully()
        {
            // Arrange
            var nombre = "Evento Pasado";
            var descripcion = "Descripción";
            var inicio = DateTime.Now.AddDays(-2);
            var fin = DateTime.Now.AddDays(-1);

            // Act
            var evento = new Evento(nombre, descripcion, inicio, fin);

            // Assert
            Assert.Equal(nombre, evento.Nombre);
            Assert.True(evento.FechaFin < DateTime.Now);
        }
    }

    public class ParticipanteTests
    {
        [Fact]
        public void Constructor_ShouldSetAllProperties()
        {
            // Arrange
            var nombre = "Usuario Test";
            var email = "usuario@test.com";
            var passwordHash = "hashedpassword";

            // Act
            var participante = new Participante(nombre, email, passwordHash);

            // Assert
            Assert.Equal(nombre, participante.Nombre);
            Assert.Equal(email, participante.Email);
            Assert.Equal(passwordHash, participante.PasswordHash);
            Assert.NotEqual(Guid.Empty, participante.Id);
        }

        [Fact]
        public void Participante_ShouldHaveUniqueId()
        {
            // Arrange
            var participante1 = new Participante("Usuario 1", "user1@test.com", "hash1");
            var participante2 = new Participante("Usuario 2", "user2@test.com", "hash2");

            // Act & Assert
            Assert.NotEqual(participante1.Id, participante2.Id);
        }
    }
}
