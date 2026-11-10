using Microsoft.EntityFrameworkCore;
using SystemX.Application.Abstractions.Persistence;
using SystemX.Infrastructure.Persistence.EfCore.Context;
using SystemX.SharedKernel.Domain;

namespace SystemX.Infrastructure.Persistence.EfCore.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IRepository{TEntity, TId}"/>.
/// </summary>
/// <typeparam name="TEntity">The aggregate root entity type.</typeparam>
/// <typeparam name="TId">The entity identifier type.</typeparam>
public class EfRepository<TEntity, TId> : EfReadRepository<TEntity, TId>, IRepository<TEntity, TId>
    where TEntity : class, IAggregateRoot, IEntity<TId>
    where TId : notnull
{
    /// <summary>
    /// Initializes a new instance of <see cref="EfRepository{TEntity, TId}"/>.
    /// </summary>
    /// <param name="context">The underlying DbContext.</param>
    public EfRepository(SystemXDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public virtual void Add(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set.Add(entity);
    }

    /// <inheritdoc />
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await Set.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        return Set.AddRangeAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public virtual void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = Context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            Set.Update(entity);
        }
        // If entity is already tracked, no-op: ChangeTracker.DetectChanges() will detect modifications.
    }

    /// <inheritdoc />
    public virtual void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set.Remove(entity);
    }

    /// <inheritdoc />
    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        Set.RemoveRange(entities);
    }
}
