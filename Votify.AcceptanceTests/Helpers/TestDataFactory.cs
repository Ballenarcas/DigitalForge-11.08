using Votify.Infrastructure.Persistence.Entities;

namespace Votify.AcceptanceTests.Helpers
{
    /// <summary>
    /// Factory para crear datos de prueba
    /// </summary>
    public static class TestDataFactory
    {
        public static VotacionEntity CrearVotacionEstandar(
            string nombre = "Votación Test",
            int limiteProyectos = 2,
            DateTime? inicio = null,
            DateTime? fin = null)
        {
            return new VotacionEntity
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Tipo = "ESTANDAR",
                FechaInicio = inicio ?? DateTime.UtcNow.AddHours(-1),
                FechaFin = fin ?? DateTime.UtcNow.AddHours(1),
                LimiteProyectos = limiteProyectos,
                PermiteComentarios = true
            };
        }

        public static VotacionEntity CrearVotacionAnonima(
            string nombre = "Votación Anónima Test",
            int limiteProyectos = 2,
            DateTime? inicio = null,
            DateTime? fin = null)
        {
            return new VotacionEntity
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Tipo = "ANONIMA",
                FechaInicio = inicio ?? DateTime.UtcNow.AddHours(-1),
                FechaFin = fin ?? DateTime.UtcNow.AddHours(1),
                LimiteProyectos = limiteProyectos,
                PermiteComentarios = false
            };
        }

        public static ProyectoEntity CrearProyecto(
            string nombre = "Proyecto Test",
            string descripcion = "Descripción test")
        {
            return new ProyectoEntity
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Descripcion = descripcion,
                Categoria_Id = null,
                Equipo_Id = null
            };
        }

        public static VotoEntity CrearVoto(
            Guid votacionId,
            Guid proyectoId,
            Guid? votanteId = null)
        {
            return new VotoEntity
            {
                Id = Guid.NewGuid(),
                VotacionId = votacionId,
                ProyectoId = proyectoId,
                VotanteId = votanteId,
                Fecha = DateTime.UtcNow
            };
        }
    }
}
