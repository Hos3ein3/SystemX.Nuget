namespace SystemX.Contracts.Pagination;

/// <summary>
/// Data transfer object representing a paginated response payload with metadata.
/// </summary>
/// <typeparam name="T">The response item type.</typeparam>
/// <param name="Items">The items in the current page.</param>
/// <param name="Page">The current 1-based page number.</param>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="TotalCount">The total count of items across all pages.</param>
/// <param name="TotalPages">The total number of pages.</param>
/// <param name="HasPreviousPage">Whether a previous page exists.</param>
/// <param name="HasNextPage">Whether a subsequent page exists.</param>
public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
