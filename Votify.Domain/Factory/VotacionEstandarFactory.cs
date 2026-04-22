using Votify.Domain.Entities;

namespace Votify.Domain.Factory
{
    public class VotacionEstandarFactory : VotacionFactory
    {
        public override Votacion Crear(string nombre, DateTime inicio, DateTime fin, int limite, bool comentarios, bool esAnonima = false)
        {
            return new VotacionEstandar(nombre, inicio, fin, limite, comentarios, esAnonima);
        }
    }
}
