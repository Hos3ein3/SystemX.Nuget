namespace SystemX.Infrastructure.Localization.Strategies;

/// <summary>
/// Resolves culture from the Accept-Language or x-culture request headers, ordering languages by quality value (q-factor).
/// </summary>
public sealed class AcceptLanguageStrategy : ICultureResolutionStrategy
{
    private readonly LocalizationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="AcceptLanguageStrategy"/> class.
    /// </summary>
    /// <param name="options">The localization options.</param>
    public AcceptLanguageStrategy(LocalizationOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public int Order => 400;

    /// <inheritdoc />
    public ValueTask<string?> ResolveAsync(CultureResolutionContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Headers.TryGetValue("x-culture", out var xCulture) && !string.IsNullOrWhiteSpace(xCulture))
        {
            return ValueTask.FromResult<string?>(xCulture);
        }

        if (context.Headers.TryGetValue("Accept-Language", out var acceptLanguage) && !string.IsNullOrWhiteSpace(acceptLanguage))
        {
            var candidates = CultureMatcher.ParseAcceptLanguage(acceptLanguage);
            var matched = CultureMatcher.Match(candidates, _options.SupportedCultures, string.Empty);
            if (!string.IsNullOrEmpty(matched))
            {
                return ValueTask.FromResult<string?>(matched);
            }
        }

        return ValueTask.FromResult<string?>(null);
    }
}
