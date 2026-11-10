namespace SystemX.SharedKernel.MultiTenancy;

/// <summary>
/// Marks an entity as shared across all tenants, always residing in the host store and never partitioned or filtered.
/// </summary>
public interface ISharedEntity
{
}
