namespace SystemX.SharedKernel.Pagination;

/// <summary>
/// Represents a subset of items retrieved via cursor-based (keyset) pagination.
/// </summary>
/// <typeparam name="T">The type of elements in the page.</typeparam>
/// <param name="Items">The items in the current page.</param>
/// <param name="NextCursor">The opaque cursor string for fetching the subsequent page, or <see langword="null"/> if no more items exist.</param>
/// <param name="HasNext">A value indicating whether more items are available.</param>
public sealed record CursorPagedList<T>(IReadOnlyList<T> Items, string? NextCursor, bool HasNext);
