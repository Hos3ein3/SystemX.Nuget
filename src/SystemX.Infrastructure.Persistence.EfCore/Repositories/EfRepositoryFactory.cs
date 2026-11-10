using System.Collections.Concurrent;
using SystemX.Application.Abstractions.Persistence;
using SystemX.Infrastructure.Persistence.EfCore.Context;
using SystemX.SharedKernel.Domain;

namespace SystemX.Infrastructure.Persistence.EfCore.Repositories;

/// <summary>
/// Factory that creates and caches repository instances bound to a single unit of work (<see cref="SystemXDbContext"/>).
/// </summary>
public sealed class EfRepositoryFactory : IRepositoryFactory
{
    private readonly SystemXDbContext _context;
    private readonly ConcurrentDictionary<(Type Entity, Type Id), object> _repositories = new();

    /// <summary>
    /// Initializes a new instance of <see cref="EfRepositoryFactory"/>.
    /// </summary>
    /// <param name="context">The underlying DbContext.</param>
    public EfRepositoryFactory(SystemXDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public IRepository<TEntity, TId> GetRepository<TEntity, TId>()
        where TEntity : class, IAggregateRoot, IEntity<TId>
        where TId : notnull
    {
        return (IRepository<TEntity, TId>)_repositories.GetOrAdd(
            (typeof(TEntity), typeof(TId)),
            _ => new EfRepository<TEntity, TId>(_context));
    }

    /// <inheritdoc />
    public IReadRepository<TEntity, TId> GetReadRepository<TEntity, TId>()
        where TEntity : class, IAggregateRoot, IEntity<TId>
        where TId : notnull
    {
        return GetRepository<TEntity, TId>();
    }
}
