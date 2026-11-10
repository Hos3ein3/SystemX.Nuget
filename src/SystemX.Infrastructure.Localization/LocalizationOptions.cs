namespace SystemX.Infrastructure.Localization;

/// <summary>
/// Configuration options for SystemX localization and culture resolution.
/// </summary>
public sealed class LocalizationOptions
{
    private IReadOnlyList<string> _supportedCultures = ["en"];

    /// <summary>
    /// Gets or sets the default fallback culture. Default is "en".
    /// </summary>
    public string DefaultCulture { get; set; } = "en";

    /// <summary>
    /// Gets or sets the list of supported culture names. Must be non-empty.
    /// </summary>
    public IReadOnlyList<string> SupportedCultures
    {
        get => _supportedCultures;
        set
        {
            if (value == null || value.Count == 0)
            {
                throw new ArgumentException("Supported cultures cannot be null or empty.", nameof(value));
            }
            _supportedCultures = value;
        }
    }

    /// <summary>
    /// Gets or sets the query parameter key used to resolve culture. Default is "culture".
    /// </summary>
    public string QueryKey { get; set; } = "culture";

    /// <summary>
    /// Gets or sets the cookie name used to resolve culture. Default is ".SystemX.Culture".
    /// </summary>
    public string CookieName { get; set; } = ".SystemX.Culture";

    /// <summary>
    /// Gets or sets the claim type used to resolve culture. Default is "locale".
    /// </summary>
    public string ClaimType { get; set; } = "locale";

    /// <summary>
    /// Gets or sets the tenant metadata property key used to resolve culture. Default is "culture".
    /// </summary>
    public string TenantPropertyKey { get; set; } = "culture";

    /// <summary>
    /// Gets or sets a value indicating whether to add the Content-Language response header. Default is true.
    /// </summary>
    public bool AddContentLanguageHeader { get; set; } = true;
}
