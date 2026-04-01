namespace Votify.Domain.Entities
{
    public class VotoAnonimo : Voto
    {
        public VotoAnonimo(string proyectoId, string votacionId)
            : base(proyectoId, null, votacionId) { }

        public override string Tipo() => "ANONIMO";
    }
}