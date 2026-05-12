using System;

namespace Votify.Domain.Entities
{
    public class Equipo
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public DateTime CreatedAt { get; set; }

        public Equipo(string nombre, Guid? id = null, DateTime? createdAt = null)
        {
            Id = id ?? Guid.Empty;
            Nombre = nombre;
            CreatedAt = createdAt ?? DateTime.UtcNow;
        }
    }
}
