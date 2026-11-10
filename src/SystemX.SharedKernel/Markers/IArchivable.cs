namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Marks an entity that supports archiving (soft archival state excluded by default).
/// </summary>
public interface IArchivable
{
    /// <summary>
    /// Gets a value indicating whether the entity is archived.
    /// </summary>
    bool IsArchived { get; }

    /// <summary>
    /// Gets the UTC instant when the entity was archived, or <see langword="null"/> if active.
    /// </summary>
    DateTimeOffset? ArchivedAt { get; }

    /// <summary>
    /// Marks the entity as archived at the specified instant.
    /// </summary>
    /// <param name="archivedAt">The archival timestamp.</param>
    void Archive(DateTimeOffset archivedAt);

    /// <summary>
    /// Restores the entity from archived state.
    /// </summary>
    void Unarchive();
}
