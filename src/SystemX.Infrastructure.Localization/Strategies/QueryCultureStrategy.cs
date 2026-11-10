namespace SystemX.Infrastructure.Localization.Strategies;

/// <summary>
/// Resolves culture from the incoming query string parameters.
/// </summary>
public sealed class QueryCultureStrategy : ICultureResolutionStrategy
{
    private readonly LocalizationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCultureStrategy"/> class.
    /// </summary>
    /// <param name="options">The localization options.</param>
    public QueryCultureStrategy(LocalizationOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public int Order => 100;

    /// <inheritdoc />
    public ValueTask<string?> ResolveAsync(CultureResolutionContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Query.TryGetValue(_options.QueryKey, out var culture) && !string.IsNullOrWhiteSpace(culture))
        {
            return ValueTask.FromResult<string?>(culture);
        }

        return ValueTask.FromResult<string?>(null);
    }
}
