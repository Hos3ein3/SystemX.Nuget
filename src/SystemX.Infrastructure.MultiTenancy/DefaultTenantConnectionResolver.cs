using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy;

/// <summary>
/// Default implementation of <see cref="ITenantConnectionResolver"/>.
/// </summary>
public sealed class DefaultTenantConnectionResolver : ITenantConnectionResolver
{
    /// <inheritdoc />
    public string GetConnectionString(TenantInfo tenant, string name = "Default")
    {
        ArgumentNullException.ThrowIfNull(tenant);

        if (!string.IsNullOrWhiteSpace(tenant.ConnectionString))
        {
            return tenant.ConnectionString;
        }

        if (tenant.Properties != null && tenant.Properties.TryGetValue($"ConnectionString_{name}", out string? namedConn) && !string.IsNullOrWhiteSpace(namedConn))
        {
            return namedConn;
        }

        throw new InvalidOperationException($"No connection string configured for tenant '{tenant.Id.Value}'.");
    }
}
