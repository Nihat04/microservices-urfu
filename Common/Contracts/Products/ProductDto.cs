using System.ComponentModel.DataAnnotations;

namespace Common.Contracts.Products;

public record ProductResponse(
    Guid Id,
    string Name,
    decimal Price,
    int Available,
    DateTime CreatedAt
);

public record ProductCreateRequest(
    [MinLength(3), MaxLength(200)] string Name,
    [Range(1, 1000000)] decimal Price,
    [Range(0, int.MaxValue)] int Available
);

public record ProductUpdateRequest(
    Guid Id,
    [MinLength(3), MaxLength(200)] string Name,
    [Range(1, 1000000)] decimal Price
);
