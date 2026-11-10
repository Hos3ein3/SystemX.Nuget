namespace SystemX.Infrastructure.Localization.Strategies;

/// <summary>
/// Resolves culture from an incoming cookie value.
/// </summary>
public sealed class CookieCultureStrategy : ICultureResolutionStrategy
{
    private readonly LocalizationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CookieCultureStrategy"/> class.
    /// </summary>
    /// <param name="options">The localization options.</param>
    public CookieCultureStrategy(LocalizationOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public int Order => 200;

    /// <inheritdoc />
    public ValueTask<string?> ResolveAsync(CultureResolutionContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Cookies.TryGetValue(_options.CookieName, out var culture) && !string.IsNullOrWhiteSpace(culture))
        {
            return ValueTask.FromResult<string?>(culture);
        }

        return ValueTask.FromResult<string?>(null);
    }
}
