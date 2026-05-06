using Votify.Domain.Entities;

namespace Votify.Domain.Factory
{
    public class VotacionRecuentoVotosFactory : VotacionFactory
    {
        public override Votacion Crear(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool comentariosObligatorios, Guid eventoId, bool esAnonima = false)
        {
            return new VotacionRecuentoVotos(nombre, inicio, fin, limite, comentarios, comentariosObligatorios, eventoId, esAnonima);
        }
    }
}
