using SystemX.SharedKernel.Domain;

namespace SystemX.SharedKernel.Specifications;

/// <summary>
/// A specification for querying an entity by its strongly-typed or primitive identifier.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
public sealed class ByIdSpecification<T, TId> : Specification<T>
    where T : class, IEntity<TId>
    where TId : notnull
{
    /// <summary>
    /// Initializes a new instance of <see cref="ByIdSpecification{T, TId}"/> matching the given ID.
    /// </summary>
    /// <param name="id">The identifier to match.</param>
    public ByIdSpecification(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Where(entity => entity.Id.Equals(id));
    }
}
