using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Provides access to the currently resolved tenant in the execution context.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets a value indicating whether a tenant has been successfully resolved for the current context.
    /// </summary>
    bool IsResolved { get; }

    /// <summary>
    /// Gets the resolved tenant metadata, or <see langword="null"/> if unauthenticated, host, or not resolved.
    /// </summary>
    TenantInfo? Tenant { get; }

    /// <summary>
    /// Gets the strongly-typed tenant identifier, or <see langword="null"/> for host or unauthenticated contexts.
    /// </summary>
    TenantId? TenantId { get; }

    /// <summary>
    /// Gets the current tenant metadata, throwing <see cref="TenantNotResolvedException"/> if not resolved.
    /// </summary>
    /// <returns>The non-null <see cref="TenantInfo"/>.</returns>
    /// <exception cref="TenantNotResolvedException">Thrown when no tenant is resolved.</exception>
    TenantInfo RequireTenant();
}
