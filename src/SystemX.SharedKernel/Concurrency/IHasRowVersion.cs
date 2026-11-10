namespace SystemX.SharedKernel.Concurrency;

/// <summary>
/// Marks an entity that uses a database-generated row version (such as SQL Server rowversion / timestamp).
/// </summary>
public interface IHasRowVersion : IConcurrencySafe
{
    /// <summary>
    /// Gets the database-generated binary row version.
    /// </summary>
    byte[] RowVersion { get; }
}
