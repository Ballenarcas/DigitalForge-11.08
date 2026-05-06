namespace Votify.Domain.Entities
{
    public class Criterio
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VotacionId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Estrellas";
        public decimal Peso { get; set; }
    }
}
