using System;

namespace Votify.Domain.Entities
{
    public class Participante
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Guid? EquipoId { get; set; }
    
        public Participante(string nombre, string email, string passwordHash, Guid? equipoId = null)
        {
            Nombre = nombre;
            Email = email;
            PasswordHash = passwordHash;
            EquipoId = equipoId;
        }   
    }

}