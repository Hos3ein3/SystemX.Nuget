namespace SystemX.Application.Abstractions.Persistence;

/// <summary>
/// Represents an active database transaction scope that can be committed or rolled back.
/// </summary>
public interface ITransactionScope : IAsyncDisposable
{
    /// <summary>
    /// Commits the active transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the active transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
