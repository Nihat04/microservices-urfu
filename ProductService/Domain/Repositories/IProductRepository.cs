using ProductService.Domain.Common;
using ProductService.Domain.Entities;

namespace ProductService.Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task<IReadOnlyList<Product>> GetByIdsAsReadOnlyAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct
    );
}
