namespace Votify.Domain.Entities
{
    public class VotoEstandar : Voto
    {
        public VotoEstandar(string proyectoId, string votanteId, string votacionId)
            : base(proyectoId, votanteId, votacionId) { }

        public override string Tipo() => "ESTANDAR";
    }
}