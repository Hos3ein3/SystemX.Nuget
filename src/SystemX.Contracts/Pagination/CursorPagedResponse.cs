namespace SystemX.Contracts.Pagination;

/// <summary>
/// Data transfer object representing a response payload from cursor-based (keyset) pagination.
/// </summary>
/// <typeparam name="T">The response item type.</typeparam>
/// <param name="Items">The items in the current page.</param>
/// <param name="NextCursor">The opaque cursor string for fetching the next page, or <see langword="null"/> if no more pages exist.</param>
/// <param name="HasNext">A value indicating whether more items are available.</param>
public sealed record CursorPagedResponse<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,
    bool HasNext);
