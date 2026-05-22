namespace Votify.Domain.Entities
{
    using Votify.Domain.Estado;

    public abstract class Votacion
    {
        public Guid Id { get; set; }
        public string Nombre { get; }
        public DateTime FechaInicio { get; }
        public DateTime FechaFin { get; }
        public int LimiteProy { get; }
        public bool Comentarios { get; }
        public bool ComentariosObligatorios { get; }
        public string Tipo { get; }
        public bool EsAnonima { get; }
        public Guid EventoId { get; set; }
        public string? ImagenUrl { get; set; }

        private IEstadoVotacion _estado;
        public IEstadoVotacion Estado => _estado;

        protected internal Votacion(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool comentariosObligatorios, string tipo, bool esAnonima, Guid eventoId, string? imagenUrl = null, IEstadoVotacion? estadoInicial = null)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            FechaInicio = inicio;
            FechaFin = fin;
            LimiteProy = limite;
            Comentarios = comentarios;
            ComentariosObligatorios = comentariosObligatorios;
            Tipo = tipo;
            EsAnonima = esAnonima;
            EventoId = eventoId;
            ImagenUrl = imagenUrl;
            _estado = estadoInicial ?? new EstadoActiva();
        }

        public void CambiarEstado(IEstadoVotacion nuevoEstado)
        {
            _estado = nuevoEstado ?? throw new ArgumentNullException(nameof(nuevoEstado));
        }

        public void Pausar() => _estado.PausarVotacion(this);
        public void Detener() => _estado.FinalizarVotacion(this);
        public void Abrir() => _estado.IniciarVotacion(this);
        public void Reanudar() => _estado.ReanudarVotacion(this);

        public void ValidarVoto()
        {
            if (DateTime.UtcNow < FechaInicio || DateTime.UtcNow > FechaFin)
                throw new InvalidOperationException("La votacion no esta dentro del periodo permitido.");
            _estado.ValidarVoto(this);
        }
    }
}