using System.Globalization;

namespace SystemX.Infrastructure.Localization;

/// <summary>
/// Internal culture matching utility that matches requested culture candidates against supported cultures.
/// Match hierarchy: exact match, parent culture, same two-letter ISO language, fallback to default.
/// Never throws for unknown or invalid cultures.
/// </summary>
internal static class CultureMatcher
{
    public static string Match(string? candidateOrAcceptLanguage, IReadOnlyList<string> supportedCultures, string defaultCulture)
    {
        if (string.IsNullOrWhiteSpace(candidateOrAcceptLanguage))
        {
            return defaultCulture;
        }

        // Check if candidate is an Accept-Language header format
        if (candidateOrAcceptLanguage.Contains(';') || candidateOrAcceptLanguage.Contains(',') ||
            candidateOrAcceptLanguage.StartsWith("Accept-Language", StringComparison.OrdinalIgnoreCase))
        {
            var candidates = ParseAcceptLanguage(candidateOrAcceptLanguage);
            return Match(candidates, supportedCultures, defaultCulture);
        }

        return Match([candidateOrAcceptLanguage], supportedCultures, defaultCulture);
    }

    public static string Match(IEnumerable<string> candidates, IReadOnlyList<string> supportedCultures, string defaultCulture)
    {
        if (supportedCultures == null || supportedCultures.Count == 0)
        {
            return defaultCulture;
        }

        foreach (var rawCandidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(rawCandidate))
            {
                continue;
            }

            var candidate = rawCandidate.Trim();

            // 1. Exact match (case-insensitive)
            var exact = supportedCultures.FirstOrDefault(s => string.Equals(s, candidate, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                return exact;
            }

            // 2. Parent culture (nl-NL -> nl)
            var parent = GetParentCultureName(candidate);
            while (!string.IsNullOrEmpty(parent))
            {
                var parentMatch = supportedCultures.FirstOrDefault(s => string.Equals(s, parent, StringComparison.OrdinalIgnoreCase));
                if (parentMatch != null)
                {
                    return parentMatch;
                }
                parent = GetParentCultureName(parent);
            }

            // 3. Supported culture with the same two-letter language
            var twoLetter = GetTwoLetterLanguage(candidate);
            if (!string.IsNullOrEmpty(twoLetter))
            {
                var langMatch = supportedCultures.FirstOrDefault(s =>
                    string.Equals(s, twoLetter, StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith(twoLetter + "-", StringComparison.OrdinalIgnoreCase));
                if (langMatch != null)
                {
                    return langMatch;
                }
            }
        }

        // 4. Default culture fallback
        return defaultCulture;
    }

    public static IReadOnlyList<string> ParseAcceptLanguage(string? headerValue)
    {
        if (string.IsNullOrWhiteSpace(headerValue))
        {
            return Array.Empty<string>();
        }

        string raw = headerValue.Trim();
        if (raw.StartsWith("Accept-Language:", StringComparison.OrdinalIgnoreCase))
        {
            raw = raw["Accept-Language:".Length..].Trim();
        }

        var list = new List<(string Language, double Quality)>();
        var parts = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            var subParts = part.Split(';', StringSplitOptions.TrimEntries);
            var lang = subParts[0];
            double quality = 1.0;
            for (int i = 1; i < subParts.Length; i++)
            {
                var qPart = subParts[i];
                if (qPart.StartsWith("q=", StringComparison.OrdinalIgnoreCase) &&
                    double.TryParse(qPart[2..], NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedQ))
                {
                    quality = parsedQ;
                }
            }
            list.Add((lang, quality));
        }

        return list.OrderByDescending(x => x.Quality).Select(x => x.Language).ToList();
    }

    private static string? GetParentCultureName(string cultureName)
    {
        try
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            if (!string.IsNullOrEmpty(culture.Parent.Name))
            {
                return culture.Parent.Name;
            }
        }
        catch (CultureNotFoundException)
        {
            // Ignore culture not found and fall back to string parsing
        }

        int dashIndex = cultureName.LastIndexOf('-');
        if (dashIndex > 0)
        {
            return cultureName[..dashIndex];
        }

        return null;
    }

    private static string? GetTwoLetterLanguage(string cultureName)
    {
        try
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            if (!string.IsNullOrEmpty(culture.TwoLetterISOLanguageName) && culture.TwoLetterISOLanguageName != "iv")
            {
                return culture.TwoLetterISOLanguageName;
            }
        }
        catch (CultureNotFoundException)
        {
            // Ignore culture not found and fall back to string parsing
        }

        int dashIndex = cultureName.IndexOf('-');
        if (dashIndex > 0)
        {
            return cultureName[..dashIndex];
        }

        return cultureName.Length == 2 ? cultureName : null;
    }
}
