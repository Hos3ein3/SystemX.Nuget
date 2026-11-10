using Microsoft.EntityFrameworkCore.Storage;
using SystemX.Application.Abstractions.Persistence;

namespace SystemX.Infrastructure.Persistence.EfCore.Transactions;

internal sealed class EfTransactionScope : ITransactionScope
{
    private readonly IDbContextTransaction _transaction;
    private bool _disposed;

    public EfTransactionScope(IDbContextTransaction transaction)
    {
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
    }

    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        _transaction.CommitAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken = default) =>
        _transaction.RollbackAsync(cancellationToken);

    public ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return ValueTask.CompletedTask;
        }

        _disposed = true;
        return _transaction.DisposeAsync();
    }
}
