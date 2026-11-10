using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Defines operations for provisioning tenant storage infrastructure (e.g. creating databases, schemas, or running migrations).
/// </summary>
public interface ITenantProvisioner
{
    /// <summary>
    /// Provisions the storage structure and applies migrations for the specified tenant.
    /// </summary>
    /// <param name="tenant">The tenant to provision.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the provisioning operation.</returns>
    Task ProvisionAsync(TenantInfo tenant, CancellationToken ct = default);
}
