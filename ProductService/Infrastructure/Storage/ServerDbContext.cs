using Microsoft.EntityFrameworkCore;

namespace ProductService.Infrastructure.Storage;

public class ServerDbContext(DbContextOptions<ServerDbContext> contextOptions)
    : DbContext(contextOptions)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServerDbContext).Assembly);
    }
}
