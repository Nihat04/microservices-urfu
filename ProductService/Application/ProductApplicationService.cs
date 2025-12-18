using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;

namespace ProductService.Application;

public interface IProductApplicationService
{
    Task<Product?> GetProductsByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Product?>> GetProductsByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct
    );
}

public sealed class ProductApplicationService(IProductRepository productRepository)
    : IProductApplicationService
{
    public async Task<Product?> GetProductsByIdAsync(Guid id, CancellationToken ct) =>
        await productRepository.GetByIdAsync(new(id), ct);

    public async Task<IReadOnlyList<Product?>> GetProductsByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct
    ) =>
        await productRepository.GetByIdsAsReadOnlyAsync(
            [.. ids.Select(id => new ProductId(id))],
            ct
        );
}
