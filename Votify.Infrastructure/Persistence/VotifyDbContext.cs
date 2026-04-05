using Microsoft.EntityFrameworkCore;
using Votify.Infrastructure.Persistence.Entities;

namespace Votify.Infrastructure.Persistence
{

public class VotifyDbContext : DbContext
{
    public VotifyDbContext(DbContextOptions<VotifyDbContext> options) : base(options) { }

    public DbSet<VotacionEntity> Votaciones { get; set; }
    public DbSet<ProyectoEntity> Proyectos { get; set; }
    public DbSet<VotoEntity> Votos { get; set; }
    public DbSet<ComentarioEntity> Comentarios { get; set; }

}
}