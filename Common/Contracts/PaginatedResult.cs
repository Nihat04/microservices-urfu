namespace Common.Contracts;

public class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public Pagination Pagination { get; }
    public int TotalCount { get; }

    public PaginatedResult(List<T> items, int currentPage, int pageSize, int totalItems)
        : this(items, new(currentPage, pageSize), totalItems) { }

    public PaginatedResult(IReadOnlyList<T> items, Pagination pagination, int totalItems)
    {
        if (totalItems < 0)
            throw new ArgumentException("Total items must be positive", nameof(totalItems));

        Items = items ?? throw new ArgumentNullException(nameof(items));
        Pagination = pagination;
        TotalCount = totalItems;
    }
}
