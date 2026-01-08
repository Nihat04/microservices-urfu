using System.Text.Json.Serialization;

namespace Common.Contracts;

public class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public Pagination Pagination { get; }
    public int TotalCount { get; }

    [JsonConstructor]
    public PaginatedResult(IReadOnlyList<T> items, Pagination pagination, int totalCount)
    {
        if (totalCount < 0)
            throw new ArgumentException("Total items must be positive", nameof(totalCount));

        Items = items ?? throw new ArgumentNullException(nameof(items));
        Pagination = pagination;
        TotalCount = totalCount;
    }

    // старый конструктор для удобства
    public PaginatedResult(List<T> items, int currentPage, int pageSize, int totalItems)
        : this(items, new Pagination(currentPage, pageSize), totalItems) { }
}