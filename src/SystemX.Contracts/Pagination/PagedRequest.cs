namespace SystemX.Contracts.Pagination;

/// <summary>
/// Data transfer object representing pagination parameters in service requests.
/// </summary>
/// <param name="Page">The 1-based page number.</param>
/// <param name="PageSize">The requested number of items per page.</param>
/// <param name="SortBy">The optional property name for sorting.</param>
/// <param name="SortDescending">Whether sorting should be applied descending.</param>
public sealed record PagedRequest(int Page = 1, int PageSize = 20, string? SortBy = null, bool SortDescending = false);
