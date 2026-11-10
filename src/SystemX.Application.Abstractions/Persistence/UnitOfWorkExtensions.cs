namespace SystemX.Application.Abstractions.Persistence;

/// <summary>
/// Provides extension methods for <see cref="IUnitOfWork"/>.
/// </summary>
public static class UnitOfWorkExtensions
{
    /// <summary>
    /// Executes an asynchronous operation inside a database transaction scope.
    /// </summary>
    /// <param name="unitOfWork">The unit of work managing transactions.</param>
    /// <param name="operation">The asynchronous operation to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task ExecuteInTransactionAsync(
        this IUnitOfWork unitOfWork,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(operation);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        await operation(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Executes an asynchronous operation that returns a value inside a database transaction scope.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="unitOfWork">The unit of work managing transactions.</param>
    /// <param name="operation">The asynchronous operation to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public static async Task<T> ExecuteInTransactionAsync<T>(
        this IUnitOfWork unitOfWork,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(operation);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        var result = await operation(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return result;
    }
}
