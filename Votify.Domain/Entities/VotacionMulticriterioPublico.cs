namespace Votify.Domain.Entities
{
    public class VotacionMulticriterioPublico : Votacion
    {
        public VotacionMulticriterioPublico(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool comentariosObligatorios, Guid eventoId, bool esAnonima = false, string? imagenUrl = null)
            : base(nombre, inicio, fin, limite, comentarios, comentariosObligatorios, "MULTICRITERIO_PUBLICO", esAnonima, eventoId, imagenUrl) { }
    }
}