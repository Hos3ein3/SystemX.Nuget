using System.Data.Common;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Application.Abstractions.Persistence;
using SystemX.Infrastructure.Persistence.EfCore.Configuration;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.Persistence.EfCore.Connections;

/// <summary>
/// Resolves database connection strings and creates raw connections without exposing EF Core.
/// </summary>
public sealed class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly SystemXEfCoreOptions _options;
    private readonly ITenantConnectionResolver? _resolver;
    private readonly ITenantContext? _ambientTenantContext;

    /// <summary>
    /// Initializes a new instance of <see cref="DatabaseConnectionFactory"/>.
    /// </summary>
    /// <param name="options">The EF Core options.</param>
    /// <param name="resolver">The optional tenant connection resolver.</param>
    /// <param name="ambientTenantContext">The optional ambient tenant context.</param>
    public DatabaseConnectionFactory(
        SystemXEfCoreOptions options,
        ITenantConnectionResolver? resolver = null,
        ITenantContext? ambientTenantContext = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _resolver = resolver;
        _ambientTenantContext = ambientTenantContext;
    }

    /// <inheritdoc />
    public string GetConnectionString(TenantInfo? tenant = null, string name = ConnectionStringResolver.DefaultName)
    {
        var target = tenant ?? _ambientTenantContext?.Tenant;
        return ConnectionStringResolver.Resolve(_options, _resolver, target, name);
    }

    /// <inheritdoc />
    public DbConnection CreateConnection(TenantInfo? tenant = null, string name = ConnectionStringResolver.DefaultName)
    {
        if (_options.CreateConnection is null)
        {
            throw new InvalidOperationException(
                "CreateConnection delegate is not configured on SystemXEfCoreOptions. A provider package must set it.");
        }

        var cs = GetConnectionString(tenant, name);
        return _options.CreateConnection(cs);
    }

    /// <inheritdoc />
    public async ValueTask<DbConnection> OpenConnectionAsync(
        TenantInfo? tenant = null,
        string name = ConnectionStringResolver.DefaultName,
        CancellationToken cancellationToken = default)
    {
        var connection = CreateConnection(tenant, name);
        try
        {
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }
}
