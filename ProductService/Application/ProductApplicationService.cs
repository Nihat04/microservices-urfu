using System.Linq.Expressions;
using System.Text;
using Common.Contracts;
using Common.Contracts.Products;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Infrastructure;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;

namespace ProductService.Application;

public interface IProductApplicationService
{
    Task<Product?> GetProductByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Product>> GetProductsByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct
    );
    Task<(IReadOnlyList<Product> items, int count)> GetProductsAsync(
        Pagination pagination,
        ProductSortBy sortBy,
        SortOrder sortOrder,
        ProductFilterDto? filter,
        CancellationToken ct
    );
    Task<Result<Product>> CreateProduct(ProductCreateRequest request, CancellationToken ct);
    Task<Result<Product>> RemoveProduct(Guid id, CancellationToken ct);
    Task<Result<Product>> UpdateProduct(
        Guid id,
        ProductUpdateRequest updateDto,
        CancellationToken ct
    );
    Task<Result<Product>> BookProduct(Guid id, int quantity, CancellationToken ct);
    Task<Result<Product>> CancelBookingProduct(Guid id, int quantity, CancellationToken ct);
    Task<Result<Product>> ShipProduct(Guid id, int quantity, CancellationToken ct);
    Task<Result<Product>> AddProduct(Guid id, int quantity, CancellationToken ct);
}

public sealed class ProductApplicationService(IProductRepository productRepository)
    : IProductApplicationService
{
    public async Task<Result<Product>> CreateProduct(
        ProductCreateRequest request,
        CancellationToken ct
    )
    {
        var product = new Product(new Guid(), request.Name, request.Price, request.Available, 0);
        await productRepository.AddAsync(product, ct);
        await productRepository.UnitOfWork.SaveChangesAsync(ct);
        return product;
    }

    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken ct) =>
        await productRepository.FindAsync([id], ct);

    public Task<IReadOnlyList<Product>> GetProductsByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct
    ) => productRepository.ListAsync(p => ids.Contains(p.Id), ct);

    public async Task<(IReadOnlyList<Product> items, int count)> GetProductsAsync(
        Pagination pagination,
        ProductSortBy orderBy,
        SortOrder sortOrder,
        ProductFilterDto? filter,
        CancellationToken ct
    )
    {
        var orderByExpression = GetOrderByExpression(orderBy);
        var products = await productRepository.QueryAsync(
            (products) =>
            {
                products = ApplyFilter(products, filter);
                if (sortOrder == SortOrder.Ascending)
                    products = products.OrderBy(orderByExpression);
                else
                    products = products.OrderByDescending(orderByExpression);
                var start = pagination.GetStart();
                return products.Skip(start).Take(pagination.PageSize);
            },
            ct
        );
        var count = await productRepository.CountQueryAsync(
            products => ApplyFilter(products, filter),
            ct
        );
        return (products, count);
    }

    public async Task<Result<Product>> UpdateProduct(
        Guid id,
        ProductUpdateRequest updateDto,
        CancellationToken ct
    ) =>
        await TryUpdateProductAndSave(
            id,
            p => p.UpdateDetails(updateDto.Name, updateDto.Price),
            ct
        );

    public async Task<Result<Product>> BookProduct(Guid id, int quantity, CancellationToken ct) =>
        await TryUpdateProductAndSave(id, p => p.Book(quantity), ct);

    public async Task<Result<Product>> CancelBookingProduct(
        Guid id,
        int quantity,
        CancellationToken ct
    ) => await TryUpdateProductAndSave(id, p => p.CancelBooking(quantity), ct);

    public async Task<Result<Product>> ShipProduct(Guid id, int quantity, CancellationToken ct) =>
        await TryUpdateProductAndSave(id, p => p.Ship(quantity), ct);

    public async Task<Result<Product>> AddProduct(Guid id, int quantity, CancellationToken ct) =>
        await TryUpdateProductAndSave(id, p => p.Add(quantity), ct);

    public async Task<Result<Product>> RemoveProduct(Guid id, CancellationToken ct)
    {
        var product = await productRepository.FindAsync([id], ct);
        if (product == null)
            return Error.NotFound($"Product with id {id} not found");
        await productRepository.RemoveAsync(product, ct);
        await productRepository.UnitOfWork.SaveChangesAsync(ct);
        return product;
    }

    private async Task<Result<Product>> TryUpdateProductAndSave(
        Guid id,
        Action<Product> updateAction,
        CancellationToken ct
    )
    {
        var product = await productRepository.FindAsync([id], ct);
        if (product == null)
            return Error.NotFound($"Product with id {id} not found");
        try
        {
            updateAction(product);
        }
        catch (Exception e)
        {
            return Error.Failure(e.Message);
        }
        await productRepository.UnitOfWork.SaveChangesAsync(ct);
        return product;
    }

    private static Expression<Func<Product, object>> GetOrderByExpression(ProductSortBy orderBy) =>
        orderBy switch
        {
            ProductSortBy.Name => p => p.Name,
            ProductSortBy.Price => p => p.Price,
            ProductSortBy.CreatedDate => p => p.CreatedAt,
            _ => throw new ArgumentOutOfRangeException(nameof(orderBy)),
        };

    private static IQueryable<Product> ApplyFilter(
        IQueryable<Product> query,
        ProductFilterDto? filter
    )
    {
        if (filter == null || filter.IsFilterEmpty())
            return query;
        if (!string.IsNullOrEmpty(filter.Name))
        {
            var search = $"%{EscapeForLike(filter.Name)}%";
            query = query.Where(p => EF.Functions.ILike(p.Name, search)); // TODO: change impl
        }
        if (filter.PriceFrom.HasValue)
            query = query.Where(p => p.Price >= filter.PriceFrom);
        if (filter.PriceTo.HasValue)
            query = query.Where(p => p.Price <= filter.PriceTo);
        return query;
    }

    private static string EscapeForLike(string input, char escapeChar = '\\')
    {
        var charsToEscape = new[] { '%', '_', '[', ']', escapeChar };
        var sb = new StringBuilder();
        foreach (var c in input)
        {
            if (charsToEscape.Contains(c))
                sb.Append(escapeChar);
            sb.Append(c);
        }
        return sb.ToString();
    }
}
