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
    public DbSet<EventoEntity> Eventos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data for events
        modelBuilder.Entity<EventoEntity>().HasData(
            new EventoEntity 
            { 
                Id = Guid.Parse("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"),
                Nombre = "Hackathon Anual 2026", 
                Descripcion = "Evento de programación de 48 horas para crear soluciones innovadoras.",
                FechaInicio = new DateTime(2026, 4, 26, 10, 0, 0, DateTimeKind.Utc),
                FechaFin = new DateTime(2026, 4, 29, 18, 0, 0, DateTimeKind.Utc),
                ImagenUrl = "images/hackathon-cover.png"
            },
            new EventoEntity 
            { 
                Id = Guid.Parse("f6e5d4c3-b2a1-4f5e-9d8c-7b6a5e4d3c2b"),
                Nombre = "Feria de Proyectos PSW", 
                Descripcion = "Presentación de proyectos finales de la asignatura Proyectos de Software.",
                FechaInicio = new DateTime(2026, 5, 5, 9, 0, 0, DateTimeKind.Utc),
                FechaFin = new DateTime(2026, 5, 10, 20, 0, 0, DateTimeKind.Utc),
                ImagenUrl = "images/feria-cover.png"
            }
        );
    }

}
}
