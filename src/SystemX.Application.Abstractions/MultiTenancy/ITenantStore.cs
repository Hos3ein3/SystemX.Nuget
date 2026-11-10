using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Defines read operations for looking up tenant configuration and metadata.
/// </summary>
public interface ITenantStore
{
    /// <summary>
    /// Looks up a tenant by its strongly-typed primary identifier.
    /// </summary>
    /// <param name="id">The tenant ID.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The tenant information if found; otherwise <see langword="null"/>.</returns>
    ValueTask<TenantInfo?> FindByIdAsync(TenantId id, CancellationToken ct = default);

    /// <summary>
    /// Looks up a tenant by an alternate or external identifier (e.g. subdomain, client code).
    /// </summary>
    /// <param name="identifier">The external identifier string.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The tenant information if found; otherwise <see langword="null"/>.</returns>
    ValueTask<TenantInfo?> FindByIdentifierAsync(string identifier, CancellationToken ct = default);

    /// <summary>
    /// Returns a list of all configured tenants.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A read-only collection of tenants.</returns>
    ValueTask<IReadOnlyList<TenantInfo>> ListAsync(CancellationToken ct = default);
}
