namespace SystemX.SharedKernel.MultiTenancy;

/// <summary>An object that belongs to one tenant.</summary>
public interface ITenantScoped
{
    /// <summary>Gets the tenant id.</summary>
    string TenantId { get; }

    /// <summary>Assigns the tenant. Called by infrastructure only.</summary>
    void AssignTenant(string tenantId);
}
