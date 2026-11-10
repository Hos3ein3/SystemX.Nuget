using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Pagination;
using SystemX.SharedKernel.Specifications;

namespace SystemX.Application.Abstractions.Persistence;

/// <summary>
/// Defines read-only repository operations using specifications for an aggregate root.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
public interface IReadRepository<T> where T : class, IAggregateRoot
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <typeparam name="TId">The identifier type.</typeparam>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity if found; otherwise <see langword="null"/>.</returns>
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

    /// <summary>
    /// Returns the first entity matching the specification or <see langword="null"/>.
    /// </summary>
    /// <param name="specification">The query specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity or <see langword="null"/>.</returns>
    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first projected result matching the specification or <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="specification">The query specification with projection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The projected result or default.</returns>
    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities matching the specification.
    /// </summary>
    /// <param name="specification">The query specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only list of entities.</returns>
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all projected results matching the specification.
    /// </summary>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="specification">The query specification with projection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only list of projected results.</returns>
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paginated list of entities matching the specification.
    /// </summary>
    /// <param name="specification">The query specification.</param>
    /// <param name="pageRequest">The pagination parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="PagedList{T}"/> containing the matching page of entities.</returns>
    Task<PagedList<T>> ListPagedAsync(ISpecification<T> specification, PageRequest pageRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the entities matching the specification.
    /// </summary>
    /// <param name="specification">The query specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The total number of matching entities.</returns>
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether any entity matches the specification.
    /// </summary>
    /// <param name="specification">The query specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see langword="true"/> if at least one entity matches; otherwise <see langword="false"/>.</returns>
    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
}
