namespace Votify.Domain.Entities
{
    public abstract class Voto
    {
        public string ProyectoId { get; }
        public string? VotanteId { get; }
        public string VotacionId { get; }

        protected Voto(string proyectoId, string? votanteId, string votacionId)
        {
            ProyectoId = proyectoId;
            VotanteId = votanteId;
            VotacionId = votacionId;
        }

        public abstract string Tipo();
    }
}