using System.Collections.Concurrent;
using SystemX.Application.Abstractions.Caching;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy.Stores;

/// <summary>
/// Caching decorator for <see cref="ITenantStore"/>. Uses <see cref="ICacheService"/> if available,
/// or falls back to an internal thread-safe in-process cache with configurable TTL.
/// </summary>
public sealed class CachedTenantStore : ITenantStore
{
    private readonly ITenantStore _innerStore;
    private readonly ICacheService? _cacheService;
    private readonly TimeSpan _ttl;

    // In-process fallback cache
    private readonly ConcurrentDictionary<string, CacheEntry> _localCache = new(StringComparer.OrdinalIgnoreCase);

    private sealed record CacheEntry(TenantInfo? Tenant, DateTimeOffset ExpiresAt);

    /// <summary>
    /// Initializes a new instance of <see cref="CachedTenantStore"/>.
    /// </summary>
    /// <param name="innerStore">The underlying tenant store.</param>
    /// <param name="cacheService">Optional distributed cache service.</param>
    /// <param name="ttl">Cache time-to-live (defaults to 15 minutes).</param>
    public CachedTenantStore(
        ITenantStore innerStore,
        ICacheService? cacheService = null,
        TimeSpan? ttl = null)
    {
        _innerStore = innerStore ?? throw new ArgumentNullException(nameof(innerStore));
        _cacheService = cacheService;
        _ttl = ttl ?? TimeSpan.FromMinutes(15);
    }

    /// <inheritdoc />
    public async ValueTask<TenantInfo?> FindByIdAsync(TenantId id, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        string cacheKey = $"systemx:tenant:id:{id.Value}";

        if (_cacheService != null)
        {
            TenantInfo? cached = await _cacheService.GetAsync<TenantInfo>(cacheKey, ct).ConfigureAwait(false);
            if (cached != null)
            {
                return cached;
            }
        }
        else if (TryGetLocal(cacheKey, out TenantInfo? localCached))
        {
            return localCached;
        }

        TenantInfo? tenant = await _innerStore.FindByIdAsync(id, ct).ConfigureAwait(false);
        if (tenant != null)
        {
            if (_cacheService != null)
            {
                await _cacheService.SetAsync(cacheKey, tenant, _ttl, ct).ConfigureAwait(false);
            }
            else
            {
                SetLocal(cacheKey, tenant);
            }
        }

        return tenant;
    }

    /// <inheritdoc />
    public async ValueTask<TenantInfo?> FindByIdentifierAsync(string identifier, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return null;
        }

        string cacheKey = $"systemx:tenant:identifier:{identifier.Trim()}";

        if (_cacheService != null)
        {
            TenantInfo? cached = await _cacheService.GetAsync<TenantInfo>(cacheKey, ct).ConfigureAwait(false);
            if (cached != null)
            {
                return cached;
            }
        }
        else if (TryGetLocal(cacheKey, out TenantInfo? localCached))
        {
            return localCached;
        }

        TenantInfo? tenant = await _innerStore.FindByIdentifierAsync(identifier, ct).ConfigureAwait(false);
        if (tenant != null)
        {
            if (_cacheService != null)
            {
                await _cacheService.SetAsync(cacheKey, tenant, _ttl, ct).ConfigureAwait(false);
            }
            else
            {
                SetLocal(cacheKey, tenant);
            }
        }

        return tenant;
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<TenantInfo>> ListAsync(CancellationToken ct = default) =>
        _innerStore.ListAsync(ct);

    private bool TryGetLocal(string key, out TenantInfo? tenant)
    {
        if (_localCache.TryGetValue(key, out CacheEntry? entry))
        {
            if (entry.ExpiresAt > DateTimeOffset.UtcNow)
            {
                tenant = entry.Tenant;
                return true;
            }

            _localCache.TryRemove(key, out _);
        }

        tenant = null;
        return false;
    }

    private void SetLocal(string key, TenantInfo? tenant)
    {
        _localCache[key] = new CacheEntry(tenant, DateTimeOffset.UtcNow.Add(_ttl));
    }
}
