namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Defines a single localized translation entry.
/// </summary>
public interface ITranslation
{
    /// <summary>
    /// Gets the ISO culture identifier (for example "en-US", "nl-NL", or "fa-IR").
    /// </summary>
    string Culture { get; }
}
