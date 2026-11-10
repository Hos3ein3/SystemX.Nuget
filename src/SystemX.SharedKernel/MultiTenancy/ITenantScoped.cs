namespace SystemX.SharedKernel.MultiTenancy;

/// <summary>
/// Marks an entity that belongs to exactly one tenant.
/// </summary>
public interface ITenantScoped
{
    /// <summary>
    /// Gets the tenant identifier.
    /// </summary>
    TenantId TenantId { get; }

    /// <summary>
    /// Assigns the tenant identifier. Called by infrastructure interceptors.
    /// </summary>
    /// <param name="tenantId">The tenant identifier to assign.</param>
    void AssignTenant(TenantId tenantId);
}
