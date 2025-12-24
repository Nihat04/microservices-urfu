using System.ComponentModel.DataAnnotations;

namespace Common.Contracts.Products;

public record ProductFilterDto(
    [MaxLength(200)] string? Name = null,
    [Range(0, 1000000)] decimal? PriceFrom = null,
    [Range(0, 1000000)] decimal? PriceTo = null
)
{
    public bool IsFilterEmpty() =>
        string.IsNullOrEmpty(Name) && PriceFrom == null && PriceTo == null;
};
