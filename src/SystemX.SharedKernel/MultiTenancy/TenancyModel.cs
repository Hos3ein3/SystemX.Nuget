namespace SystemX.SharedKernel.MultiTenancy;

/// <summary>
/// Specifies how tenant data is isolated across storage layers.
/// </summary>
public enum TenancyModel
{
    /// <summary>
    /// No multi-tenancy. Single tenant application or host-only shared data.
    /// </summary>
    None = 0,

    /// <summary>
    /// Shared database and shared schema. Rows are partitioned by a tenant identifier column.
    /// </summary>
    Row = 1,

    /// <summary>
    /// Shared database where each tenant has its own isolated schema.
    /// </summary>
    Schema = 2,

    /// <summary>
    /// Dedicated database per tenant.
    /// </summary>
    Database = 3
}
