namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Marks an entity that supports explicit display ordering.
/// </summary>
public interface IOrderable
{
    /// <summary>
    /// Gets the display sort order value (lower numbers appear first).
    /// </summary>
    int SortOrder { get; }
}
