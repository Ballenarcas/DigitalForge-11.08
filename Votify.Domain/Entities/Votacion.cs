namespace Votify.Domain.Entities
{
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
        public EstadoVotacion Estado { get; set; }
        
        private IEstadoVotacion _estado => Estado switch
        {
            EstadoVotacion.Abierta => new EstadoActiva(),
            EstadoVotacion.Pausada => new EstadoPausada(),
            EstadoVotacion.Detenida => new EstadoFinalizada(),
            _ => new EstadoActiva()
        };

        protected Votacion(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool comentariosObligatorios, string tipo, bool esAnonima, Guid eventoId)
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
            Estado = EstadoVotacion.Abierta;
        }

        public void Pausar()
        {
            _estado.PausarVotacion(this);
        }

        public void Detener()
        {
            _estado.FinalizarVotacion(this);
        }

        public void Abrir()
        {
            _estado.IniciarVotacion(this);
        }
        public void Reanudar()
        {
            _estado.ReanudarVotacion(this);
        }

        public void ValidarVoto()
        {
            if (DateTime.Now < FechaInicio || DateTime.Now > FechaFin)
            {
                throw new InvalidOperationException("La votación no está dentro del período permitido.");
            }
            _estado.ValidarVoto(this);
        }
    }
}
