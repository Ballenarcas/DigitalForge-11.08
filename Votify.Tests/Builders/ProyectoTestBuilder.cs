#nullable enable
using System;
using Votify.Domain.Builders;
using Votify.Domain.Entities;

namespace Votify.Tests.Builders
{
    /// <summary>
    /// Builder de prueba para Proyecto con valores por defecto seguros.
    /// </summary>
    public class ProyectoTestBuilder
    {
        private string _nombre = "Proyecto de Prueba";
        private string _descripcion = "Descripción de proyecto de prueba";
        private string? _equipoId = Guid.NewGuid().ToString();
        private Guid _votacionId = Guid.NewGuid();
        private string? _imagenUrl;
        private string? _id;

        public ProyectoTestBuilder ConNombre(string nombre)
        {
            _nombre = nombre;
            return this;
        }

        public ProyectoTestBuilder ConDescripcion(string descripcion)
        {
            _descripcion = descripcion;
            return this;
        }

        public ProyectoTestBuilder DelEquipo(string? equipoId)
        {
            _equipoId = equipoId;
            return this;
        }

        public ProyectoTestBuilder ConEquipoId(string? equipoId)
        {
            _equipoId = equipoId;
            return this;
        }

        public ProyectoTestBuilder DeLaVotacion(Guid votacionId)
        {
            _votacionId = votacionId;
            return this;
        }

        public ProyectoTestBuilder ConVotacionId(Guid votacionId)
        {
            _votacionId = votacionId;
            return this;
        }

        public ProyectoTestBuilder ConImagen(string? imagenUrl)
        {
            _imagenUrl = imagenUrl;
            return this;
        }

        public ProyectoTestBuilder ConId(string? id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Construye la entidad Proyecto para pruebas.
        /// </summary>
        public Proyecto Build()
        {
            var builder = new ProyectoBuilder()
                .ConNombre(_nombre)
                .ConDescripcion(_descripcion)
                .DelEquipo(_equipoId)
                .DeLaVotacion(_votacionId)
                .ConImagen(_imagenUrl)
                .ConId(_id);

            return builder.Build();
        }
    }
}
