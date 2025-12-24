using System.ComponentModel.DataAnnotations;

namespace Common.Contracts;

public record Pagination(
    [Range(0, int.MaxValue, ErrorMessage = "Current page must be positive")] int Page = 0,
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")] int PageSize = 100
)
{
    public int GetStart() => PageSize * Page;
}
