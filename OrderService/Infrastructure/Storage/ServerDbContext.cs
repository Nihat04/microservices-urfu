using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Common;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Storage;

public class ServerDbContext(DbContextOptions<ServerDbContext> contextOptions)
    : DbContext(contextOptions),
        IUnitOfWork
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
