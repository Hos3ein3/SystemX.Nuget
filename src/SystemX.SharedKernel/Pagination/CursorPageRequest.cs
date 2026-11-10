namespace SystemX.SharedKernel.Pagination;

/// <summary>
/// Encapsulates parameters for cursor-based (keyset) pagination.
/// </summary>
/// <param name="Cursor">The opaque cursor string representing the position of the last seen item, or <see langword="null"/> for the first page.</param>
/// <param name="PageSize">The number of items to return.</param>
public sealed record CursorPageRequest(string? Cursor, int PageSize);
