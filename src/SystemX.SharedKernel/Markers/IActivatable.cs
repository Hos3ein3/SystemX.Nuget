namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Marks an entity that can be activated or deactivated.
/// </summary>
public interface IActivatable
{
    /// <summary>
    /// Gets a value indicating whether the entity is currently active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Activates the entity.
    /// </summary>
    void Activate();

    /// <summary>
    /// Deactivates the entity.
    /// </summary>
    void Deactivate();
}
