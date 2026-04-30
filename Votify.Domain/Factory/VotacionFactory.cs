using Votify.Domain.Entities;

namespace Votify.Domain.Factory
{
    public abstract class VotacionFactory
    {
        public abstract Votacion Crear(
            string nombre,
            DateTime inicio,
            DateTime fin,
            int limite,
            bool comentarios,
            bool comentariosObligatorios,
            Guid eventoId,
            bool esAnonima = false
        );
    }
}
