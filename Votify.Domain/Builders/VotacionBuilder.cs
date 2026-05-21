#nullable enable
using System;
using Votify.Domain.Entities;

namespace Votify.Domain.Builders
{
    /// <summary>
    /// Builder para la construcción fluida y validada de entidades Votacion.
    /// Encapsula la lógica de post-construcción (auto-pausado) y garantiza
    /// estados válidos en el dominio.
    /// </summary>
    public class VotacionBuilder
    {
        private string _nombre = default!;
        private DateTime _inicio;
        private DateTime _fin;
        private int _limiteProy;
        private bool _comentarios;
        private bool _comentariosObligatorios;
        private bool _esAnonima;
        private Guid _eventoId;
        private string _tipo = default!;
        private string? _imagenUrl;

        /// <summary>
        /// Establece el nombre de la votación.
        /// </summary>
        public VotacionBuilder ConNombre(string nombre)
        {
            _nombre = nombre;
            return this;
        }

        /// <summary>
        /// Establece el período de la votación (inicio y fin).
        /// </summary>
        public VotacionBuilder ConPeriodo(DateTime inicio, DateTime fin)
        {
            _inicio = inicio;
            _fin = fin;
            return this;
        }

        /// <summary>
        /// Establece el límite de proyectos para votar.
        /// </summary>
        public VotacionBuilder ConLimiteProyectos(int limite)
        {
            _limiteProy = limite;
            return this;
        }

        /// <summary>
        /// Establece si los comentarios están habilitados y si son obligatorios.
        /// </summary>
        public VotacionBuilder ConComentarios(bool habilitados, bool obligatorios = false)
        {
            _comentarios = habilitados;
            _comentariosObligatorios = obligatorios;
            return this;
        }

        /// <summary>
        /// Establece si la votación es anónima.
        /// </summary>
        public VotacionBuilder EsAnonima(bool anonima = false)
        {
            _esAnonima = anonima;
            return this;
        }

        /// <summary>
        /// Establece el tipo de votación (ESTANDAR o MULTICRITERIO).
        /// </summary>
        public VotacionBuilder DelTipo(string tipo)
        {
            _tipo = tipo;
            return this;
        }

        /// <summary>
        /// Establece el ID del evento asociado.
        /// </summary>
        public VotacionBuilder DelEvento(Guid eventoId)
        {
            _eventoId = eventoId;
            return this;
        }

        /// <summary>
        /// Establece la URL de la imagen.
        /// </summary>
        public VotacionBuilder ConImagen(string? imagenUrl)
        {
            _imagenUrl = imagenUrl;
            return this;
        }

        /// <summary>
        /// Construye la entidad Votacion validando todos los parámetros.
        /// Si la fecha de inicio es futura, pausa automáticamente la votación.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cuando falta algún parámetro obligatorio o las validaciones fallan.</exception>
        public Votacion Build()
        {
            ValidarEstadoDelBuilder();

            Votacion votacion = _tipo.ToUpper() switch
            {
                "ESTANDAR" => new VotacionEstandar(
                    _nombre,
                    _inicio,
                    _fin,
                    _limiteProy,
                    _comentarios,
                    _comentariosObligatorios,
                    _eventoId,
                    _esAnonima,
                    _imagenUrl),

                "MULTICRITERIO" => new VotacionMulticriterio(
                    _nombre,
                    _inicio,
                    _fin,
                    _limiteProy,
                    _comentarios,
                    _comentariosObligatorios,
                    _eventoId,
                    _esAnonima,
                    _imagenUrl),

                _ => throw new InvalidOperationException(
                    $"Tipo de votación no válido: {_tipo}. Use 'ESTANDAR' o 'MULTICRITERIO'.")
            };

            // Auto-pausar si la votación comienza en el futuro
            if (_inicio > DateTime.UtcNow)
            {
                votacion.Pausar();
            }

            return votacion;
        }

        private void ValidarEstadoDelBuilder()
        {
            if (string.IsNullOrWhiteSpace(_nombre))
                throw new InvalidOperationException("El nombre de la votación es obligatorio.");

            if (_inicio == default)
                throw new InvalidOperationException("La fecha de inicio es obligatoria.");

            if (_fin == default)
                throw new InvalidOperationException("La fecha de fin es obligatoria.");

            if (_inicio >= _fin)
                throw new InvalidOperationException("La fecha de inicio debe ser anterior a la fecha de fin.");

            if (_limiteProy <= 0)
                throw new InvalidOperationException("El límite de proyectos debe ser mayor a 0.");

            if (_eventoId == Guid.Empty)
                throw new InvalidOperationException("El ID del evento es obligatorio.");

            if (string.IsNullOrWhiteSpace(_tipo))
                throw new InvalidOperationException("El tipo de votación es obligatorio.");
        }
    }
}
