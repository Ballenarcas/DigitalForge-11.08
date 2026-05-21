#nullable enable
using System;
using Votify.Domain.Entities;

namespace Votify.Domain.Builders
{
    /// <summary>
    /// Builder para la construcción fluida y validada de entidades Evento.
    /// Elimina los parámetros opcionales mixtos y proporciona una API clara.
    /// </summary>
    public class EventoBuilder
    {
        private string _nombre = default!;
        private string _descripcion = default!;
        private DateTime _fechaInicio;
        private DateTime _fechaFin;
        private string? _imagenUrl;
        private Guid? _id;

        public EventoBuilder ConNombre(string nombre)
        {
            _nombre = nombre;
            return this;
        }

        public EventoBuilder ConDescripcion(string descripcion)
        {
            _descripcion = descripcion;
            return this;
        }

        public EventoBuilder ConFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
            return this;
        }

        public EventoBuilder ConFechaInicio(DateTime fechaInicio)
        {
            _fechaInicio = fechaInicio;
            return this;
        }

        public EventoBuilder ConFechaFin(DateTime fechaFin)
        {
            _fechaFin = fechaFin;
            return this;
        }

        public EventoBuilder ConImagen(string? imagenUrl)
        {
            _imagenUrl = imagenUrl;
            return this;
        }

        public EventoBuilder ConId(Guid id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Construye la entidad Evento validando todos los parámetros.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cuando falta algún parámetro obligatorio o las validaciones fallan.</exception>
        public Evento Build()
        {
            ValidarEstadoDelBuilder();

            return new Evento(
                _nombre,
                _descripcion,
                _fechaInicio,
                _fechaFin,
                _imagenUrl,
                _id);
        }

        private void ValidarEstadoDelBuilder()
        {
            if (string.IsNullOrWhiteSpace(_nombre))
                throw new InvalidOperationException("El nombre del evento es obligatorio.");

            if (string.IsNullOrWhiteSpace(_descripcion))
                throw new InvalidOperationException("La descripción del evento es obligatoria.");

            if (_fechaInicio == default)
                throw new InvalidOperationException("La fecha de inicio del evento es obligatoria.");

            if (_fechaFin == default)
                throw new InvalidOperationException("La fecha de fin del evento es obligatoria.");

            if (_fechaInicio >= _fechaFin)
                throw new InvalidOperationException("La fecha de inicio debe ser anterior a la fecha de fin.");
        }
    }
}
