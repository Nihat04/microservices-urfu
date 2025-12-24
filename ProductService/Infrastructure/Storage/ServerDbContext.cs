using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Common;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Storage;

public class ServerDbContext(DbContextOptions<ServerDbContext> contextOptions)
    : DbContext(contextOptions),
        IUnitOfWork
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
