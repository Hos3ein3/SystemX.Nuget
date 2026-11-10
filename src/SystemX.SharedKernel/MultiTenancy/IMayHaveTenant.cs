namespace SystemX.SharedKernel.MultiTenancy;

/// <summary>
/// Marks an entity that belongs either to a specific tenant or to the host (shared fallback).
/// </summary>
public interface IMayHaveTenant
{
    /// <summary>
    /// Gets the tenant identifier, or <see langword="null"/> if the entity belongs to the host.
    /// </summary>
    TenantId? TenantId { get; }

    /// <summary>
    /// Assigns the tenant identifier (or null for host). Called by infrastructure interceptors.
    /// </summary>
    /// <param name="tenantId">The tenant identifier to assign, or <see langword="null"/>.</param>
    void AssignTenant(TenantId? tenantId);
}
