namespace Common.Contracts;

public class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public PaginatedResult(List<T> items, int currentPage, int totalItems, int pageSize)
    {
        if (currentPage < 1)
            throw new ArgumentException(
                "Current page must be greater than 0.",
                nameof(currentPage)
            );
        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0.", nameof(pageSize));
        if (totalItems < 0)
            throw new ArgumentException(
                "Total items must be greater than or equal to 0.",
                nameof(totalItems)
            );

        var _items = items ?? throw new ArgumentNullException(nameof(items));
        var _currentPage = currentPage;
        var TotalItems = totalItems;
        var _pageSize = pageSize;
        var TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}
