using ProductService.Domain.Common;

namespace ProductService.Domain.Entities;

public readonly record struct ProductId(Guid Id);

public sealed class Product : Entity<ProductId>, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Product()
        : base(default) { }

    public Product(ProductId id, string name, decimal price)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        Name = name;
        Price = price;
        CreatedAt = DateTime.UtcNow;
    }
}
