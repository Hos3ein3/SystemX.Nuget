using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.Persistence;

/// <summary>
/// One unit of work with its repositories, for one tenant or for the host. Dispose it when done.
/// </summary>
public interface IDatabaseSession : IUnitOfWork, IAsyncDisposable
{
    /// <summary>Gets the tenant of this session, or null for the host / a non-tenant application.</summary>
    TenantInfo? Tenant { get; }

    /// <summary>Gets the repositories bound to this session.</summary>
    IRepositoryFactory Repositories { get; }

    /// <summary>Gets raw read connections bound to this session.</summary>
    IReadConnectionFactory ReadConnections { get; }
}
