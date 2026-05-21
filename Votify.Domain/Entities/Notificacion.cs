using System;

namespace Votify.Domain.Entities
{
    public class Notificacion
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? RecursoId { get; set; }
        public string? RecursoTipo { get; set; }
        public bool Leida { get; set; }
        public DateTime CreatedAt { get; set; }

        public Notificacion(Guid usuarioId, string mensaje, string tipo, string? recursoId = null, string? recursoTipo = null, Guid? id = null, DateTime? createdAt = null)
        {
            Id = id ?? Guid.NewGuid();
            UsuarioId = usuarioId;
            Mensaje = mensaje;
            Tipo = tipo;
            RecursoId = recursoId;
            RecursoTipo = recursoTipo;
            Leida = false;
            CreatedAt = createdAt ?? DateTime.UtcNow;
        }
    }
}