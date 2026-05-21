#nullable enable
using System;
using Votify.Domain.Builders;
using Votify.Domain.Entities;

namespace Votify.Tests.Builders
{
    /// <summary>
    /// Builder de prueba para Evento con valores por defecto seguros.
    /// Utiliza DateTime.UtcNow para garantizar consistencia en tests.
    /// </summary>
    public class EventoTestBuilder
    {
        private string _nombre = "Evento de Prueba";
        private string _descripcion = "Descripción de evento de prueba";
        private DateTime _fechaInicio = DateTime.UtcNow.AddDays(1);
        private DateTime _fechaFin = DateTime.UtcNow.AddDays(2);
        private string? _imagenUrl;
        private Guid? _id;

        public EventoTestBuilder ConNombre(string nombre)
        {
            _nombre = nombre;
            return this;
        }

        public EventoTestBuilder ConDescripcion(string descripcion)
        {
            _descripcion = descripcion;
            return this;
        }

        public EventoTestBuilder ConFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
            return this;
        }

        public EventoTestBuilder ConFechaInicio(DateTime fechaInicio)
        {
            _fechaInicio = fechaInicio;
            return this;
        }

        public EventoTestBuilder ConInicio(DateTime fechaInicio)
        {
            _fechaInicio = fechaInicio;
            return this;
        }

        public EventoTestBuilder ConFechaFin(DateTime fechaFin)
        {
            _fechaFin = fechaFin;
            return this;
        }

        public EventoTestBuilder ConFin(DateTime fechaFin)
        {
            _fechaFin = fechaFin;
            return this;
        }

        public EventoTestBuilder ConImagen(string? imagenUrl)
        {
            _imagenUrl = imagenUrl;
            return this;
        }

        public EventoTestBuilder ConId(Guid id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Construye la entidad Evento para pruebas.
        /// </summary>
        public Evento Build()
        {
            var builder = new EventoBuilder()
                .ConNombre(_nombre)
                .ConDescripcion(_descripcion)
                .ConFechas(_fechaInicio, _fechaFin)
                .ConImagen(_imagenUrl);

            if (_id.HasValue)
            {
                builder.ConId(_id.Value);
            }

            return builder.Build();
        }
    }
}
