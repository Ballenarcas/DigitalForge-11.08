using Votify.Domain.Entities;

namespace Votify.Domain.Factory
{
    public class VotoMulticriterioAnonimoFactory : VotoFactory
    {
        public override Voto Crear(string proyectoId, string votacionId, string? votanteId = null)
        {
            return new VotoMulticriterioAnonimo(proyectoId, votacionId);
        }
    }
}