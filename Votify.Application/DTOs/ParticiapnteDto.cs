
    public class ParticipanteDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid? EquipoId { get; set; }
    }