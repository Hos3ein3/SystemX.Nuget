using System.Data.Common;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.Persistence;

/// <summary>Resolves connection strings and creates raw connections. Never exposes EF Core.</summary>
public interface IDatabaseConnectionFactory
{
    /// <summary>
    /// Gets the connection string for a tenant (or the current tenant when null) and a connection name.
    /// The connection string contains sensitive credentials and must NEVER be logged or exposed to clients.
    /// </summary>
    /// <param name="tenant">The target tenant metadata, or null to resolve for the current ambient tenant or host.</param>
    /// <param name="name">The connection string identifier name.</param>
    /// <returns>The resolved raw connection string.</returns>
    string GetConnectionString(TenantInfo? tenant = null, string name = "Default");

    /// <summary>Creates a closed connection.</summary>
    /// <param name="tenant">The target tenant metadata, or null to resolve for the current ambient tenant or host.</param>
    /// <param name="name">The connection string identifier name.</param>
    /// <returns>A new closed <see cref="DbConnection"/>.</returns>
    DbConnection CreateConnection(TenantInfo? tenant = null, string name = "Default");

    /// <summary>Creates and opens a connection. The caller owns and disposes it.</summary>
    /// <param name="tenant">The target tenant metadata, or null to resolve for the current ambient tenant or host.</param>
    /// <param name="name">The connection string identifier name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An open <see cref="DbConnection"/>.</returns>
    ValueTask<DbConnection> OpenConnectionAsync(
        TenantInfo? tenant = null, string name = "Default", CancellationToken cancellationToken = default);
}
