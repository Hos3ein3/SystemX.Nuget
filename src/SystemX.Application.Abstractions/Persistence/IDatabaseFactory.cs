using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.Persistence;

/// <summary>
/// Creates database sessions outside the normal request scope: background jobs, tenant loops, provisioning, host administration.
/// </summary>
public interface IDatabaseFactory
{
    /// <summary>Creates a session for the current tenant (or the host when tenancy is not used).</summary>
    Task<IDatabaseSession> CreateSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a session for a specific tenant, whatever the current tenant is.</summary>
    Task<IDatabaseSession> CreateSessionForTenantAsync(TenantInfo tenant, CancellationToken cancellationToken = default);

    /// <summary>Creates a session for the host database. Tenancy is ignored (no tenant filters).</summary>
    Task<IDatabaseSession> CreateHostSessionAsync(CancellationToken cancellationToken = default);
}
