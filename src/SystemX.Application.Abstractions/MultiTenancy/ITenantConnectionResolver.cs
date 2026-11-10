using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Resolves the database connection string for a tenant under the Database tenancy model.
/// </summary>
public interface ITenantConnectionResolver
{
    /// <summary>
    /// Gets the connection string for the specified tenant and logical connection name.
    /// </summary>
    /// <param name="tenant">The tenant information.</param>
    /// <param name="name">The logical connection name (defaults to "Default").</param>
    /// <returns>The resolved connection string.</returns>
    string GetConnectionString(TenantInfo tenant, string name = "Default");
}
