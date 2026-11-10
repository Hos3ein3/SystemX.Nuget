using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy;

/// <summary>
/// Configuration options for tenant resolution, stores, and enforcement policies.
/// </summary>
public sealed class MultiTenancyOptions
{
    /// <summary>
    /// Gets or sets the default isolation model when not explicitly configured on a tenant.
    /// Default is <see cref="TenancyModel.Row"/>.
    /// </summary>
    public TenancyModel DefaultModel { get; set; } = TenancyModel.Row;

    /// <summary>
    /// Gets or sets a value indicating whether a tenant context is required for requests.
    /// Default is <see langword="true"/>.
    /// </summary>
    public bool RequireTenant { get; set; } = true;

    /// <summary>
    /// Gets or sets the HTTP header name for resolving tenant identifiers.
    /// Default is "X-Tenant-Id".
    /// </summary>
    public string HeaderName { get; set; } = "X-Tenant-Id";

    /// <summary>
    /// Gets or sets the JWT/User claim type for resolving tenant identifiers.
    /// Default is "tenant_id".
    /// </summary>
    public string ClaimType { get; set; } = "tenant_id";

    /// <summary>
    /// Gets or sets the query string parameter name for resolving tenant identifiers.
    /// Default is "tenant".
    /// </summary>
    public string QueryKey { get; set; } = "tenant";

    /// <summary>
    /// Gets or sets the route parameter name for resolving tenant identifiers.
    /// Default is "tenant".
    /// </summary>
    public string RouteKey { get; set; } = "tenant";

    /// <summary>
    /// Gets or sets the host domain suffix used to extract subdomains (e.g. ".example.com").
    /// </summary>
    public string? HostSuffix { get; set; }

    /// <summary>
    /// Gets or sets the format pattern used to compute database schema names for tenants under the Schema model.
    /// Default is "tenant_{0}".
    /// </summary>
    public string SchemaNameFormat { get; set; } = "tenant_{0}";

    /// <summary>
    /// Gets the collection of request path prefixes excluded from tenant resolution and enforcement (e.g. health checks, swagger).
    /// </summary>
    public IList<string> ExcludedPaths { get; } = new List<string>
    {
        "/health",
        "/healthz",
        "/alive",
        "/ready",
        "/swagger",
        "/favicon.ico"
    };
}
