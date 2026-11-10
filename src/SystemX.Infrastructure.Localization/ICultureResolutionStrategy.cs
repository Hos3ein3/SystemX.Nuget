namespace SystemX.Infrastructure.Localization;

/// <summary>
/// Defines a strategy for resolving a culture name from a <see cref="CultureResolutionContext"/>.
/// </summary>
public interface ICultureResolutionStrategy
{
    /// <summary>
    /// Gets the execution priority order (lower runs first).
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Attempts to resolve a culture name or culture candidate from the resolution context.
    /// </summary>
    /// <param name="context">The culture resolution context.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The resolved culture string if found; otherwise <see langword="null"/>.</returns>
    ValueTask<string?> ResolveAsync(CultureResolutionContext context, CancellationToken cancellationToken = default);
}
