using Votify.Domain.Entities;

namespace Votify.Domain.Factory
{
    public abstract class VotoFactory
    {
        public abstract Voto Crear(string proyectoId, string votacionId, string? votanteId = null);
    }
}
