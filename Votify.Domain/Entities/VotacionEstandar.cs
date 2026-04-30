namespace Votify.Domain.Entities
{
    public class VotacionEstandar : Votacion
    {
        public VotacionEstandar(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool comentariosObligatorios, Guid eventoId, bool esAnonima = false)
            : base(nombre, inicio, fin, limite, comentarios, comentariosObligatorios, "ESTANDAR", esAnonima, eventoId) { }
    }
}