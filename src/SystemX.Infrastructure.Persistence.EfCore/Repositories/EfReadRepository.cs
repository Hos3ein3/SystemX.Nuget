using Microsoft.EntityFrameworkCore;
using SystemX.Application.Abstractions.Persistence;
using SystemX.Infrastructure.Persistence.EfCore.Context;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Pagination;
using SystemX.SharedKernel.Specifications;

namespace SystemX.Infrastructure.Persistence.EfCore.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IReadRepository{TEntity, TId}"/>.
/// </summary>
/// <typeparam name="TEntity">The aggregate root entity type.</typeparam>
/// <typeparam name="TId">The entity identifier type.</typeparam>
public class EfReadRepository<TEntity, TId> : IReadRepository<TEntity, TId>
    where TEntity : class, IAggregateRoot, IEntity<TId>
    where TId : notnull
{
    /// <summary>
    /// Initializes a new instance of <see cref="EfReadRepository{TEntity, TId}"/>.
    /// </summary>
    /// <param name="context">The underlying DbContext.</param>
    public EfReadRepository(SystemXDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>Gets the underlying <see cref="SystemXDbContext"/>.</summary>
    protected SystemXDbContext Context { get; }

    /// <summary>Gets the <see cref="DbSet{TEntity}"/> for this entity type.</summary>
    protected DbSet<TEntity> Set => Context.Set<TEntity>();

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        // Set.FirstOrDefaultAsync(IdPredicate.Build<TEntity, TId>(id)).
        // NEVER use FindAsync: it bypasses global query filters (multi-tenancy and soft delete) and would leak cross-tenant or deleted data.
        return await Set.FirstOrDefaultAsync(IdPredicate.Build<TEntity, TId>(id), cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return SpecificationEvaluator.GetQuery(Set, specification).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return SpecificationEvaluator.GetQuery(Set, specification).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await SpecificationEvaluator.GetQuery(Set, specification).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await SpecificationEvaluator.GetQuery(Set, specification).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<PagedList<TEntity>> ListPagedAsync(
        ISpecification<TEntity> specification,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        ArgumentNullException.ThrowIfNull(pageRequest);

        var total = await SpecificationEvaluator.GetQuery(Set, specification, criteriaOnly: true, applyPaging: false)
            .CountAsync(cancellationToken);

        var items = await SpecificationEvaluator.GetQuery(Set, specification, criteriaOnly: false, applyPaging: false)
            .Skip(pageRequest.Skip)
            .Take(pageRequest.PageSize)
            .ToListAsync(cancellationToken);

        return PagedList<TEntity>.Create(items, total, pageRequest);
    }

    /// <inheritdoc />
    public virtual Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return SpecificationEvaluator.GetQuery(Set, specification, criteriaOnly: true, applyPaging: false)
            .CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return SpecificationEvaluator.GetQuery(Set, specification, criteriaOnly: true, applyPaging: false)
            .AnyAsync(cancellationToken);
    }
}
