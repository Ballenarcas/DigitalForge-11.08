namespace Votify.Domain.Entities
{
    public class VotoMulticriterioAnonimo : Voto
    {
        public VotoMulticriterioAnonimo(string proyectoId, string votacionId)
            : base(proyectoId, null, votacionId) { }

        public override string Tipo() => "MULTICRITERIO_ANONIMO";
    }
}