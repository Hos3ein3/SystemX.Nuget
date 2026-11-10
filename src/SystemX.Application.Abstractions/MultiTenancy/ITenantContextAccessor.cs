using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Provides access to and control over the ambient tenant context across asynchronous control flows.
/// </summary>
public interface ITenantContextAccessor : ITenantContext
{
    /// <summary>
    /// Sets the active tenant for the current asynchronous execution scope.
    /// Dispose the returned handle to restore the previously active tenant.
    /// </summary>
    /// <param name="tenant">The tenant to activate in this scope, or <see langword="null"/> for host context.</param>
    /// <returns>A disposable handle that reverts the tenant scope upon disposal.</returns>
    IDisposable BeginScope(TenantInfo? tenant);
}
