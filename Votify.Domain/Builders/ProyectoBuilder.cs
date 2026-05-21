#nullable enable
using System;
using Votify.Domain.Entities;

namespace Votify.Domain.Builders
{
    /// <summary>
    /// Builder para la construcción fluida y validada de entidades Proyecto.
    /// Elimina la ambigüedad de orden de parámetros y proporciona una API clara.
    /// </summary>
    public class ProyectoBuilder
    {
        private string _nombre = default!;
        private string _descripcion = default!;
        private string? _equipoId;
        private Guid _votacionId;
        private string? _imagenUrl;
        private string? _id;

        public ProyectoBuilder ConNombre(string nombre)
        {
            _nombre = nombre;
            return this;
        }

        public ProyectoBuilder ConDescripcion(string descripcion)
        {
            _descripcion = descripcion;
            return this;
        }

        public ProyectoBuilder DelEquipo(string? equipoId)
        {
            _equipoId = equipoId;
            return this;
        }

        public ProyectoBuilder DeLaVotacion(Guid votacionId)
        {
            _votacionId = votacionId;
            return this;
        }

        public ProyectoBuilder ConImagen(string? imagenUrl)
        {
            _imagenUrl = imagenUrl;
            return this;
        }

        public ProyectoBuilder ConId(string? id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Construye la entidad Proyecto validando todos los parámetros.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cuando falta algún parámetro obligatorio.</exception>
        public Proyecto Build()
        {
            ValidarEstadoDelBuilder();

            return new Proyecto(
                _nombre,
                _descripcion,
                _equipoId,
                _votacionId,
                _imagenUrl,
                _id);
        }

        private void ValidarEstadoDelBuilder()
        {
            if (string.IsNullOrWhiteSpace(_nombre))
                throw new InvalidOperationException("El nombre del proyecto es obligatorio.");

            if (string.IsNullOrWhiteSpace(_descripcion))
                throw new InvalidOperationException("La descripción del proyecto es obligatoria.");

            if (_votacionId == Guid.Empty)
                throw new InvalidOperationException("El ID de la votación es obligatorio.");
        }
    }
}
