using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Common;

namespace ProductService.Infrastructure.DataStorage;

public abstract class EFRepository<TAggregateRoot, TDbContext>(TDbContext context)
    : IRepository<TAggregateRoot>
    where TAggregateRoot : class, IAggregateRoot
    where TDbContext : DbContext
{
    private readonly DbContext _context = context;

    public Task SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
}
