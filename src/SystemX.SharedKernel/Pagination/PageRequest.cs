namespace SystemX.SharedKernel.Pagination;

/// <summary>
/// Encapsulates page number and page size parameters for paginated queries.
/// </summary>
public sealed record PageRequest
{
    /// <summary>
    /// Default page size used when not specified.
    /// </summary>
    public const int DefaultPageSize = 20;

    /// <summary>
    /// Maximum allowed page size to prevent unbounded memory usage.
    /// </summary>
    public const int MaxPageSize = 200;

    /// <summary>
    /// Gets the default pagination request (Page 1, default page size).
    /// </summary>
    public static readonly PageRequest Default = new(1, DefaultPageSize);

    /// <summary>
    /// Gets the 1-based page number.
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Gets the page size (number of items per page).
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets the optional property name used for sorting.
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Gets a value indicating whether sorting is descending.
    /// </summary>
    public bool SortDescending { get; init; }

    /// <summary>
    /// Gets the computed number of items to skip for offset-based pagination.
    /// </summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>
    /// Initializes a new instance of <see cref="PageRequest"/>.
    /// </summary>
    /// <param name="page">The 1-based page index (must be >= 1).</param>
    /// <param name="pageSize">The number of items per page (1 to 200).</param>
    /// <param name="sortBy">The optional sort property.</param>
    /// <param name="sortDescending">Whether sorting should be descending.</param>
    public PageRequest(int page = 1, int pageSize = DefaultPageSize, string? sortBy = null, bool sortDescending = false)
    {
        Page = page < 1 ? 1 : page;
        PageSize = pageSize < 1 ? DefaultPageSize : (pageSize > MaxPageSize ? MaxPageSize : pageSize);
        SortBy = sortBy;
        SortDescending = sortDescending;
    }

    /// <summary>
    /// Creates a new <see cref="PageRequest"/> clamping invalid values to valid ranges.
    /// </summary>
    /// <param name="page">The requested page number.</param>
    /// <param name="pageSize">The requested page size.</param>
    /// <param name="sortBy">The requested sort property.</param>
    /// <param name="sortDescending">Whether to sort descending.</param>
    /// <returns>A validated <see cref="PageRequest"/> instance.</returns>
    public static PageRequest Create(int? page, int? pageSize, string? sortBy = null, bool sortDescending = false)
    {
        int p = page.GetValueOrDefault(1);
        int ps = pageSize.GetValueOrDefault(DefaultPageSize);
        return new PageRequest(p, ps, sortBy, sortDescending);
    }
}
