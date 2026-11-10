namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Marks an entity that records its UTC creation timestamp.
/// </summary>
public interface IHasCreationTime
{
    /// <summary>
    /// Gets the UTC instant when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Sets the creation timestamp. Called by persistence interceptors upon insertion.
    /// </summary>
    /// <param name="createdAt">The creation timestamp.</param>
    void SetCreationTime(DateTimeOffset createdAt);
}
