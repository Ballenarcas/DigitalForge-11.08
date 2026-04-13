using Microsoft.EntityFrameworkCore;
using Votify.Infrastructure.Persistence;
using Xunit;

namespace Votify.AcceptanceTests.Helpers
{
    /// <summary>
    /// Clase base para las pruebas de aceptación que proporciona una BD en memoria
    /// </summary>
    public abstract class AcceptanceTestBase : IAsyncLifetime
    {
        protected VotifyDbContext DbContext { get; private set; } = null!;

        public virtual async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<VotifyDbContext>()
                .UseInMemoryDatabase(databaseName: $"VotifyTestDb_{Guid.NewGuid()}")
                .Options;

            DbContext = new VotifyDbContext(options);
            await DbContext.Database.EnsureCreatedAsync();
        }

        public virtual async Task DisposeAsync()
        {
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.DisposeAsync();
        }
    }
}
