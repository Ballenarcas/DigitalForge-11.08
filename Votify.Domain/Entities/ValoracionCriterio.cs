namespace Votify.Domain.Entities
{
    public class ValoracionCriterio
    {
        public Guid Id { get; set; }
        public Guid VotanteId { get; set; }
        public Guid CriterioId { get; set; }
        public Guid ProyectoId { get; set; }
        public int Valoracion { get; set; }
    }
}
