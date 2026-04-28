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
        public string Tipo { get; }
        public bool EsAnonima { get; }
        public Guid EventoId { get; set; }

        protected Votacion(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, string tipo, bool esAnonima, Guid eventoId)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            FechaInicio = inicio;
            FechaFin = fin;
            LimiteProy = limite;
            Comentarios = comentarios;
            Tipo = tipo;
            EsAnonima = esAnonima;
            EventoId = eventoId;
        }
    }
}
