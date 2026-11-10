namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Marks an entity that records its UTC modification timestamp.
/// </summary>
public interface IHasModificationTime
{
    /// <summary>
    /// Gets the UTC instant when the entity was last modified, or <see langword="null"/> if never modified.
    /// </summary>
    DateTimeOffset? ModifiedAt { get; }

    /// <summary>
    /// Sets the modification timestamp. Called by persistence interceptors upon update.
    /// </summary>
    /// <param name="modifiedAt">The modification timestamp.</param>
    void SetModificationTime(DateTimeOffset modifiedAt);
}
