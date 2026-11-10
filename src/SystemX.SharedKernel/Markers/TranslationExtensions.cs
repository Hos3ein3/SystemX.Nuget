namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Provides helper extension methods for selecting localized translations from <see cref="ITranslatable{TTranslation}"/>.
/// </summary>
public static class TranslationExtensions
{
    /// <summary>
    /// Selects the best matching translation based on requested culture, neutral culture fallback, and default fallback.
    /// Priority: exact culture ("nl-NL") -> neutral culture ("nl") -> fallback culture -> first available -> null.
    /// </summary>
    /// <typeparam name="TTranslation">The translation type.</typeparam>
    /// <param name="entity">The translatable entity.</param>
    /// <param name="culture">The requested culture code.</param>
    /// <param name="fallbackCulture">The default fallback culture code.</param>
    /// <returns>The matching translation or <see langword="null"/> if none available.</returns>
    public static TTranslation? Pick<TTranslation>(
        this ITranslatable<TTranslation> entity,
        string culture,
        string fallbackCulture)
        where TTranslation : ITranslation
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(fallbackCulture);

        IReadOnlyCollection<TTranslation> translations = entity.Translations;
        if (translations == null || translations.Count == 0)
        {
            return default;
        }

        // 1. Exact match (case-insensitive)
        TTranslation? exact = translations.FirstOrDefault(t =>
            string.Equals(t.Culture, culture, StringComparison.OrdinalIgnoreCase));
        if (exact != null)
        {
            return exact;
        }

        // 2. Neutral culture prefix (e.g. "nl" from "nl-NL")
        int dashIndex = culture.IndexOf('-', StringComparison.Ordinal);
        if (dashIndex > 0)
        {
            string neutral = culture[..dashIndex];
            TTranslation? neutralMatch = translations.FirstOrDefault(t =>
                string.Equals(t.Culture, neutral, StringComparison.OrdinalIgnoreCase) ||
                t.Culture.StartsWith(neutral + "-", StringComparison.OrdinalIgnoreCase));
            if (neutralMatch != null)
            {
                return neutralMatch;
            }
        }

        // 3. Fallback culture match
        TTranslation? fallback = translations.FirstOrDefault(t =>
            string.Equals(t.Culture, fallbackCulture, StringComparison.OrdinalIgnoreCase));
        if (fallback != null)
        {
            return fallback;
        }

        // 4. First available
        return translations.FirstOrDefault();
    }
}
