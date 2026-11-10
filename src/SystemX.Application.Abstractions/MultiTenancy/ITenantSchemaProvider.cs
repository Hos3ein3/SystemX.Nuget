using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Provides the schema name for a tenant under the Schema tenancy model.
/// </summary>
public interface ITenantSchemaProvider
{
    /// <summary>
    /// Gets the database schema name for the specified tenant.
    /// </summary>
    /// <param name="tenant">The tenant information.</param>
    /// <returns>The safe database schema name.</returns>
    string GetSchema(TenantInfo tenant);
}
