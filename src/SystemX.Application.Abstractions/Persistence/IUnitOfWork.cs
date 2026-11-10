namespace SystemX.Application.Abstractions.Persistence;

/// <summary>
/// Encapsulates a unit of work that commits pending state changes and manages database transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets a value indicating whether there is an active database transaction.
    /// </summary>
    bool HasActiveTransaction { get; }

    /// <summary>
    /// Asynchronously persists all tracked changes to the underlying database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new transaction scope.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous transaction scope handle.</returns>
    Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
