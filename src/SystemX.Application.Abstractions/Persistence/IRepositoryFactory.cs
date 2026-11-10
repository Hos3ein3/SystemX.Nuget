using SystemX.SharedKernel.Domain;

namespace SystemX.Application.Abstractions.Persistence;

/// <summary>Gives access to repositories that share one unit of work.</summary>
public interface IRepositoryFactory
{
    /// <summary>Gets a read-write repository for the aggregate. The same instance is returned for the same types.</summary>
    IRepository<TEntity, TId> GetRepository<TEntity, TId>()
        where TEntity : class, IAggregateRoot, IEntity<TId>
        where TId : notnull;

    /// <summary>Gets a read-only repository for the aggregate.</summary>
    IReadRepository<TEntity, TId> GetReadRepository<TEntity, TId>()
        where TEntity : class, IAggregateRoot, IEntity<TId>
        where TId : notnull;
}
