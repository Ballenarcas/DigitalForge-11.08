namespace Votify.Domain.Entities
{
    public class VotacionMulticriterioPublico : Votacion
    {
        public VotacionMulticriterioPublico(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool comentariosObligatorios, Guid eventoId, bool esAnonima = false)
            : base(nombre, inicio, fin, limite, true, true, "MULTICRITERIO_PUBLICO", esAnonima, eventoId) { }
    }
}