using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy;

/// <summary>
/// Provides ambient tenant context access using <see cref="AsyncLocal{T}"/> with hierarchical scope management.
/// </summary>
public sealed class TenantContextAccessor : ITenantContextAccessor
{
    private static readonly AsyncLocal<TenantInfo?> CurrentTenantHolder = new();

    /// <inheritdoc />
    public bool IsResolved => CurrentTenantHolder.Value != null;

    /// <inheritdoc />
    public TenantInfo? Tenant => CurrentTenantHolder.Value;

    /// <inheritdoc />
    public TenantId? TenantId => CurrentTenantHolder.Value?.Id;

    /// <inheritdoc />
    public TenantInfo RequireTenant()
    {
        return CurrentTenantHolder.Value ?? throw new TenantNotResolvedException();
    }

    /// <inheritdoc />
    public IDisposable BeginScope(TenantInfo? tenant)
    {
        TenantInfo? previous = CurrentTenantHolder.Value;
        CurrentTenantHolder.Value = tenant;
        return new RevertScope(previous);
    }

    private sealed class RevertScope(TenantInfo? previous) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                CurrentTenantHolder.Value = previous;
                _disposed = true;
            }
        }
    }
}
