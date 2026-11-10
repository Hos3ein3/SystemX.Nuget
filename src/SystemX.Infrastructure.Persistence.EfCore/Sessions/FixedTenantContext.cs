using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.Persistence.EfCore.Sessions;

internal sealed class FixedTenantContext : ITenantContext
{
    public FixedTenantContext(TenantInfo? tenant)
    {
        Tenant = tenant;
    }

    public TenantInfo? Tenant { get; }

    public bool IsResolved => Tenant is not null;

    public TenantId? TenantId => Tenant?.Id;

    public TenantInfo RequireTenant() =>
        Tenant ?? throw new TenantNotResolvedException();
}
