namespace SystemX.Infrastructure.Persistence.EfCore.Context;

/// <summary>
/// Data filters that can be temporarily disabled per <see cref="SystemXDbContext"/> instance.
/// </summary>
public enum DataFilter
{
    /// <summary>
    /// Soft-delete filter hiding entities where IsDeleted is <see langword="true"/>.
    /// </summary>
    SoftDelete = 0,

    /// <summary>
    /// Archived filter hiding entities where IsArchived is <see langword="true"/>.
    /// </summary>
    Archived = 1,

    /// <summary>
    /// Tenant isolation filter restricting rows to the current tenant.
    /// Disabling this filter is dangerous and should only be done for cross-tenant operations.
    /// </summary>
    Tenant = 2
}
