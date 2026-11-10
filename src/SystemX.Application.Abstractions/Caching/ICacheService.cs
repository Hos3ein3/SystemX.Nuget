namespace SystemX.Application.Abstractions.Caching;

/// <summary>
/// Abstraction for cache operations in the application layer.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached item by key or default if not found.
    /// </summary>
    /// <typeparam name="T">The cached item type.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The cached item or default.</returns>
    ValueTask<T?> GetAsync<T>(string key, CancellationToken ct = default);

    /// <summary>
    /// Sets a cached item with an optional expiration time.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The item to store.</param>
    /// <param name="expiration">The optional time-to-live expiration.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the cache set operation.</returns>
    ValueTask SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);

    /// <summary>
    /// Removes an item from the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the cache removal operation.</returns>
    ValueTask RemoveAsync(string key, CancellationToken ct = default);
}
