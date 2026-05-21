#nullable enable
using System;
using Votify.Domain.Builders;
using Votify.Domain.Entities;

namespace Votify.Tests.Builders
{
    /// <summary>
    /// Builder de prueba para Votacion con valores por defecto seguros.
    /// Utiliza DateTime.UtcNow y GUIDs para garantizar consistencia en tests.
    /// </summary>
    public class VotacionTestBuilder
    {
        private string _nombre = "Votación de Prueba";
        private DateTime _inicio = DateTime.UtcNow.AddHours(1);
        private DateTime _fin = DateTime.UtcNow.AddHours(3);
        private int _limiteProy = 5;
        private bool _comentarios = true;
        private bool _comentariosObligatorios = false;
        private bool _esAnonima = false;
        private Guid _eventoId = Guid.NewGuid();
        private string _tipo = "ESTANDAR";
        private string? _imagenUrl;

        public VotacionTestBuilder ConNombre(string nombre)
        {
            _nombre = nombre;
            return this;
        }

        public VotacionTestBuilder ConPeriodo(DateTime inicio, DateTime fin)
        {
            _inicio = inicio;
            _fin = fin;
            return this;
        }

        public VotacionTestBuilder ConInicio(DateTime inicio)
        {
            _inicio = inicio;
            return this;
        }

        public VotacionTestBuilder ConFechaInicio(DateTime inicio)
        {
            _inicio = inicio;
            return this;
        }

        public VotacionTestBuilder ConFin(DateTime fin)
        {
            _fin = fin;
            return this;
        }

        public VotacionTestBuilder ConFechaFin(DateTime fin)
        {
            _fin = fin;
            return this;
        }

        public VotacionTestBuilder ConLimiteProy(int limite)
        {
            _limiteProy = limite;
            return this;
        }

        public VotacionTestBuilder ConLimiteProyectos(int limite)
        {
            _limiteProy = limite;
            return this;
        }

        public VotacionTestBuilder ConComentarios(bool habilitados, bool obligatorios = false)
        {
            _comentarios = habilitados;
            _comentariosObligatorios = obligatorios;
            return this;
        }

        public VotacionTestBuilder ConComentariosObligatorios(bool obligatorios)
        {
            _comentariosObligatorios = obligatorios;
            return this;
        }

        public VotacionTestBuilder EsAnonima(bool anonima = true)
        {
            _esAnonima = anonima;
            return this;
        }

        public VotacionTestBuilder ConEsAnonima(bool anonima)
        {
            _esAnonima = anonima;
            return this;
        }

        public VotacionTestBuilder DelTipo(string tipo)
        {
            _tipo = tipo;
            return this;
        }

        public VotacionTestBuilder DelEvento(Guid eventoId)
        {
            _eventoId = eventoId;
            return this;
        }

        public VotacionTestBuilder ConEventoId(Guid eventoId)
        {
            _eventoId = eventoId;
            return this;
        }

        public VotacionTestBuilder ConImagen(string? imagenUrl)
        {
            _imagenUrl = imagenUrl;
            return this;
        }

        /// <summary>
        /// Construye la entidad Votacion para pruebas.
        /// </summary>
        public Votacion Build()
        {
            var builder = new VotacionBuilder()
                .ConNombre(_nombre)
                .ConPeriodo(_inicio, _fin)
                .ConLimiteProyectos(_limiteProy)
                .ConComentarios(_comentarios, _comentariosObligatorios)
                .EsAnonima(_esAnonima)
                .DelTipo(_tipo)
                .DelEvento(_eventoId)
                .ConImagen(_imagenUrl);

            return builder.Build();
        }
    }
}
