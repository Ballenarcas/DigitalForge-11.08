namespace Votify.Domain.Entities
{
    public class VotacionEstandar : Votacion
    {
        internal VotacionEstandar(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool comentariosObligatorios, Guid eventoId, bool esAnonima = false, string? imagenUrl = null)
            : base(nombre, inicio, fin, limite, comentarios, comentariosObligatorios, "ESTANDAR", esAnonima, eventoId, imagenUrl) { }
    }
}