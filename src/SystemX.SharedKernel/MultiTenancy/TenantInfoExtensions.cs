namespace SystemX.SharedKernel.MultiTenancy;

/// <summary>
/// Provides extension methods for querying the isolation strategy of a <see cref="TenantInfo"/>.
/// </summary>
public static class TenantInfoExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> if the tenant uses row-level partitioning with tenant filters.
    /// </summary>
    public static bool UsesRowFilter(this TenantInfo tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        return tenant.Model == TenancyModel.Row;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the tenant uses a dedicated database schema.
    /// </summary>
    public static bool UsesSchema(this TenantInfo tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        return tenant.Model == TenancyModel.Schema;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the tenant uses a dedicated database.
    /// </summary>
    public static bool UsesDedicatedDatabase(this TenantInfo tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        return tenant.Model == TenancyModel.Database;
    }
}
