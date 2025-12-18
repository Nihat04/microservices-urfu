using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;

namespace ProductService.Infrastructure.Storage.Repositories;

public class ProductRepository(ServerDbContext context)
    : EFRepository<Product, ServerDbContext>(context),
        IProductRepository
{
    public Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct) =>
        _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Product>> GetByIdsAsReadOnlyAsync(
        IReadOnlyCollection<ProductId> ids,
        CancellationToken ct
    ) => await _context.Products.AsNoTracking().Where(p => ids.Contains(p.Id)).ToListAsync(ct);
}
