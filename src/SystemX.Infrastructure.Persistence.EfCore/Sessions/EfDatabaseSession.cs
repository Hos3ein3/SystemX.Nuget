using Microsoft.Extensions.DependencyInjection;
using SystemX.Application.Abstractions.Persistence;
using SystemX.Infrastructure.Persistence.EfCore.Context;
using SystemX.Infrastructure.Persistence.EfCore.Repositories;
using SystemX.Infrastructure.Persistence.EfCore.Transactions;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.Persistence.EfCore.Sessions;

internal sealed class EfDatabaseSession : IDatabaseSession
{
    private readonly AsyncServiceScope _scope;
    private readonly SystemXDbContext _context;
    private bool _disposed;

    public EfDatabaseSession(AsyncServiceScope scope, SystemXDbContext context, TenantInfo? tenant)
    {
        _scope = scope;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Tenant = tenant;
        Repositories = new EfRepositoryFactory(context);
        ReadConnections = new EfReadConnectionFactory(context);
    }

    public TenantInfo? Tenant { get; }

    public IRepositoryFactory Repositories { get; }

    public IReadConnectionFactory ReadConnections { get; }

    public bool HasActiveTransaction => _context.HasActiveTransaction;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        _context.BeginTransactionAsync(cancellationToken);

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        await _context.DisposeAsync();
        await _scope.DisposeAsync();
    }
}
