using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Common.Contracts;

public class Pagination
{
    [Range(0, int.MaxValue, ErrorMessage = "Current page must be positive")]
    public int Page { get; set; } = 0;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 100;

    public Pagination() { }

    [JsonConstructor]
    public Pagination(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public int GetStart() => PageSize * Page;
}