using SystemX.SharedKernel.Domain;

namespace SystemX.Application.Abstractions.Persistence;

/// <summary>
/// Defines read and write repository operations for an aggregate root.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
public interface IRepository<T> : IReadRepository<T> where T : class, IAggregateRoot
{
    /// <summary>
    /// Adds a new aggregate root to the repository.
    /// </summary>
    /// <param name="entity">The aggregate root entity.</param>
    void Add(T entity);

    /// <summary>
    /// Updates an existing aggregate root in the repository.
    /// </summary>
    /// <param name="entity">The aggregate root entity.</param>
    void Update(T entity);

    /// <summary>
    /// Removes an aggregate root from the repository.
    /// </summary>
    /// <param name="entity">The aggregate root entity.</param>
    void Remove(T entity);
}
