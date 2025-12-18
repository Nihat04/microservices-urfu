namespace Common.Contracts.Products;

enum ProductSortBy
{
    Name,
    Price,
}

record ProductFilterDto(
    ProductSortBy? SortBy,
    SortOrder? SortOrder,
    int Page,
    int PageSize,
    string? Name = null,
    decimal? PriceFrom = null,
    decimal? PriceTo = null
);
