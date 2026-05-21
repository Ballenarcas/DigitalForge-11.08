namespace Votify.Domain.Entities
{
    public class VotacionMulticriterio : Votacion
    {
        internal VotacionMulticriterio(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool comentariosObligatorios, Guid eventoId, bool esAnonima = false, string? imagenUrl = null)
            : base(nombre, inicio, fin, limite, comentarios, comentariosObligatorios, "MULTICRITERIO", esAnonima, eventoId, imagenUrl) { }
    }
}
