using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Infrastructure.Persistence.EfCore.Configuration;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.Persistence.EfCore.Connections;

internal static class ConnectionStringResolver
{
    public const string DefaultName = "Default";

    public static string Resolve(
        SystemXEfCoreOptions options,
        ITenantConnectionResolver? resolver,
        TenantInfo? tenant,
        string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!options.IgnoreTenancy && tenant is { Model: TenancyModel.Database })
        {
            var resolved = resolver?.GetConnectionString(tenant, name);
            if (!string.IsNullOrWhiteSpace(resolved))
            {
                return resolved;
            }

            if (name.Equals(DefaultName, StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(tenant.ConnectionString))
            {
                return tenant.ConnectionString;
            }
        }

        if (name.Equals(DefaultName, StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(options.HostConnectionString))
            {
                return options.HostConnectionString;
            }

            throw new InvalidOperationException("HostConnectionString must be configured on SystemXEfCoreOptions.");
        }

        if (options.NamedConnectionStrings.TryGetValue(name, out var cs) && !string.IsNullOrWhiteSpace(cs))
        {
            return cs;
        }

        // Never include connection strings in exception messages
        throw new InvalidOperationException($"No connection string registered with name '{name}'.");
    }
}
