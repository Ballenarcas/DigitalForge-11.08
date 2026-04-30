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
            if (Estado == EstadoVotacion.Detenida)
                throw new InvalidOperationException("No se puede pausar una votación detenida.");
            Estado = EstadoVotacion.Pausada;
        }

        public void Detener()
        {
            if (Estado == EstadoVotacion.Detenida)
                throw new InvalidOperationException("La votación ya está detenida.");
            Estado = EstadoVotacion.Detenida;
        }

        public void Abrir()
        {
            if (Estado == EstadoVotacion.Abierta)
                throw new InvalidOperationException("La votación ya está abierta.");
            Estado = EstadoVotacion.Abierta;
        }
    }
}
