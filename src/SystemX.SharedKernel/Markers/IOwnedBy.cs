namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Marks an entity that is owned by a specific principal or user identifier.
/// </summary>
/// <typeparam name="TOwnerId">The owner identifier type.</typeparam>
public interface IOwnedBy<out TOwnerId> where TOwnerId : notnull
{
    /// <summary>
    /// Gets the identifier of the owning entity or principal.
    /// </summary>
    TOwnerId OwnerId { get; }
}
