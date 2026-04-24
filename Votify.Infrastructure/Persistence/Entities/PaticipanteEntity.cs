using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Infrastructure.Persistence.Entities
{
    [Table("participante")]
    public class ParticipanteEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("rol")]
        public string Rol { get; set; } = string.Empty;
        
        [Column("PasswordHash")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
