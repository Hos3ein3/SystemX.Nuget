namespace SystemX.SharedKernel.Concurrency;

/// <summary>
/// Marks an entity that uses a numeric version incremented by the application on each update.
/// Highly portable across SQL Server, PostgreSQL, and MongoDB.
/// </summary>
public interface IHasVersion : IConcurrencySafe
{
    /// <summary>
    /// Gets the current numeric version.
    /// </summary>
    long Version { get; }

    /// <summary>
    /// Increments the numeric version. Called by infrastructure interceptors upon modification.
    /// </summary>
    void IncrementVersion();
}
