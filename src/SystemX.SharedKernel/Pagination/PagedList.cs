namespace SystemX.SharedKernel.Pagination;

/// <summary>
/// Represents a paged subset of items with metadata.
/// </summary>
/// <typeparam name="T">The type of elements in the page.</typeparam>
public sealed class PagedList<T>
{
    /// <summary>
    /// Gets the items in the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Gets the current 1-based page number.
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Initializes a new instance of <see cref="PagedList{T}"/>.
    /// </summary>
    public PagedList(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        ArgumentNullException.ThrowIfNull(items);
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Creates an empty paged list for the given request.
    /// </summary>
    public static PagedList<T> Empty(PageRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new PagedList<T>(Array.Empty<T>(), request.Page, request.PageSize, 0);
    }

    /// <summary>
    /// Creates a paged list with the given items, total count, and request.
    /// </summary>
    public static PagedList<T> Create(IReadOnlyList<T> items, int totalCount, PageRequest request)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(request);
        return new PagedList<T>(items, request.Page, request.PageSize, totalCount);
    }

    /// <summary>
    /// Projects the elements of the paged list to a new form.
    /// </summary>
    /// <typeparam name="TOut">The target element type.</typeparam>
    /// <param name="map">The projection function.</param>
    /// <returns>A new <see cref="PagedList{TOut}"/> containing the mapped elements.</returns>
    public PagedList<TOut> Map<TOut>(Func<T, TOut> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        List<TOut> mapped = new(Items.Count);
        foreach (T item in Items)
        {
            mapped.Add(map(item));
        }

        return new PagedList<TOut>(mapped.AsReadOnly(), Page, PageSize, TotalCount);
    }
}
