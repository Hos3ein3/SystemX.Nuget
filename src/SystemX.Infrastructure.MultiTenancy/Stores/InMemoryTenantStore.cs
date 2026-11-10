using System.Collections.Concurrent;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy.Stores;

/// <summary>
/// In-memory implementation of <see cref="ITenantStore"/> suitable for tests and lightweight configurations.
/// </summary>
public sealed class InMemoryTenantStore : ITenantStore
{
    private readonly ConcurrentDictionary<string, TenantInfo> _tenantsById = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, TenantInfo> _tenantsByIdentifier = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of <see cref="InMemoryTenantStore"/> with optional seed tenants.
    /// </summary>
    /// <param name="tenants">The initial set of tenants.</param>
    public InMemoryTenantStore(IEnumerable<TenantInfo>? tenants = null)
    {
        if (tenants != null)
        {
            foreach (TenantInfo tenant in tenants)
            {
                AddOrUpdate(tenant);
            }
        }
    }

    /// <summary>
    /// Adds or updates a tenant in the store.
    /// </summary>
    /// <param name="tenant">The tenant information.</param>
    public void AddOrUpdate(TenantInfo tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        _tenantsById[tenant.Id.Value] = tenant;

        if (!string.IsNullOrWhiteSpace(tenant.Identifier))
        {
            _tenantsByIdentifier[tenant.Identifier] = tenant;
        }
    }

    /// <inheritdoc />
    public ValueTask<TenantInfo?> FindByIdAsync(TenantId id, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        _tenantsById.TryGetValue(id.Value, out TenantInfo? tenant);
        return ValueTask.FromResult(tenant);
    }

    /// <inheritdoc />
    public ValueTask<TenantInfo?> FindByIdentifierAsync(string identifier, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return ValueTask.FromResult<TenantInfo?>(null);
        }

        if (_tenantsByIdentifier.TryGetValue(identifier, out TenantInfo? tenant))
        {
            return ValueTask.FromResult<TenantInfo?>(tenant);
        }

        // Fallback: try finding by ID if identifier equals ID string
        _tenantsById.TryGetValue(identifier, out tenant);
        return ValueTask.FromResult(tenant);
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<TenantInfo>> ListAsync(CancellationToken ct = default)
    {
        IReadOnlyList<TenantInfo> list = _tenantsById.Values.ToList().AsReadOnly();
        return ValueTask.FromResult(list);
    }
}
