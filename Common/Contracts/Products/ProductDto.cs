namespace Common.Contracts.Products;

public record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    decimal Available,
    DateTime CreatedAt
);
