using SystemX.SharedKernel.Domain;

namespace SystemX.Application.Abstractions.Persistence;

/// <summary>
/// Defines read and write repository operations for an aggregate root.
/// </summary>
/// <typeparam name="TEntity">The aggregate root type.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
public interface IRepository<TEntity, in TId> : IReadRepository<TEntity, TId>
    where TEntity : class, IAggregateRoot, IEntity<TId>
    where TId : notnull
{
    /// <summary>
    /// Adds a new aggregate root to the repository.
    /// </summary>
    /// <param name="entity">The aggregate root entity.</param>
    void Add(TEntity entity);

    /// <summary>
    /// Asynchronously adds a new aggregate root to the repository.
    /// </summary>
    /// <param name="entity">The aggregate root entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds multiple aggregate roots to the repository.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing aggregate root in the repository.
    /// </summary>
    /// <param name="entity">The aggregate root entity.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an aggregate root from the repository.
    /// </summary>
    /// <param name="entity">The aggregate root entity.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Removes multiple aggregate roots from the repository.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    void RemoveRange(IEnumerable<TEntity> entities);
}
