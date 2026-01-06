using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Common;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Storage;

public class ServerDbContext(DbContextOptions<ServerDbContext> contextOptions)
    : DbContext(contextOptions),
        IUnitOfWork
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Order>().Navigation(o => o.Items).AutoInclude();
    }
}
