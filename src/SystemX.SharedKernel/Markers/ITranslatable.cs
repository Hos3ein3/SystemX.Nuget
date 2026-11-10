namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Marks an entity that supports multilingual translations.
/// </summary>
/// <typeparam name="TTranslation">The translation entity type.</typeparam>
public interface ITranslatable<out TTranslation> where TTranslation : ITranslation
{
    /// <summary>
    /// Gets the collection of available localized translations.
    /// </summary>
    IReadOnlyCollection<TTranslation> Translations { get; }
}
