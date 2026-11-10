namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Defines a strategy for extracting a tenant identifier from a <see cref="TenantResolutionContext"/>.
/// </summary>
public interface ITenantResolutionStrategy
{
    /// <summary>
    /// Gets the execution priority order (lower runs first).
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Attempts to extract a raw tenant identifier string from the given resolution context.
    /// </summary>
    /// <param name="context">The resolution context.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The raw tenant identifier string if resolved; otherwise <see langword="null"/>.</returns>
    ValueTask<string?> ResolveIdentifierAsync(TenantResolutionContext context, CancellationToken ct = default);
}
