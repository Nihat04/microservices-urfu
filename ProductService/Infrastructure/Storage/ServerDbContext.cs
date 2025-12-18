using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Storage;

public class ServerDbContext(DbContextOptions<ServerDbContext> contextOptions)
    : DbContext(contextOptions)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServerDbContext).Assembly);
    }
}
