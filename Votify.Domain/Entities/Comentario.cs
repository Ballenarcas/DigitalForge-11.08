using System;

namespace Votify.Domain.Entities
{
    public class Comentario
    {
        public string Texto { get; set; } = string.Empty;
        public Guid? AutorId { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
