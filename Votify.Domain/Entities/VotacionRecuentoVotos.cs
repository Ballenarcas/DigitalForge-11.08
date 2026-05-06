namespace Votify.Domain.Entities
{
    public class VotacionRecuentoVotos : Votacion
    {
        public VotacionRecuentoVotos(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool comentariosObligatorios, Guid eventoId, bool esAnonima = false)
            : base(nombre, inicio, fin, limite, comentarios, comentariosObligatorios, "Recuento de votos", esAnonima, eventoId) { }
    }
}
