namespace ProductService.Application.Extensions;

using Common.Contracts.Products;
using ProductService.Domain.Entities;

public static class ProductExtensions
{
    public static ProductResponse ToDto(this Product product) =>
        new(product.Id, product.Name, product.Price, product.AvailableQuantity, product.CreatedAt);
}
