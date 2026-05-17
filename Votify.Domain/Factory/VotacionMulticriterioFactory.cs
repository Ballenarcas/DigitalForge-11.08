using Votify.Domain.Entities;

namespace Votify.Domain.Factory
{
    public class VotacionMulticriterioFactory : VotacionFactory
    {
        public override Votacion Crear(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool comentariosObligatorios, Guid eventoId, bool esAnonima = false, string? imagenUrl = null)
        {
            return new VotacionMulticriterio(nombre, inicio, fin, limite, comentarios, comentariosObligatorios, eventoId, esAnonima, imagenUrl);
        }
    }
}
