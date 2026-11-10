namespace SystemX.Infrastructure.Localization.Strategies;

/// <summary>
/// Resolves culture from an authenticated user claim.
/// </summary>
public sealed class ClaimCultureStrategy : ICultureResolutionStrategy
{
    private readonly LocalizationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimCultureStrategy"/> class.
    /// </summary>
    /// <param name="options">The localization options.</param>
    public ClaimCultureStrategy(LocalizationOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public int Order => 300;

    /// <inheritdoc />
    public ValueTask<string?> ResolveAsync(CultureResolutionContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Claims.TryGetValue(_options.ClaimType, out var culture) && !string.IsNullOrWhiteSpace(culture))
        {
            return ValueTask.FromResult<string?>(culture);
        }

        return ValueTask.FromResult<string?>(null);
    }
}
