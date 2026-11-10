using SystemX.SharedKernel.Domain;

namespace SystemX.SharedKernel.Concurrency;

/// <summary>
/// Abstract base aggregate root implementing numeric optimistic concurrency tracking (<see cref="IHasVersion"/>).
/// </summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public abstract class VersionedAggregateRoot<TId> : AggregateRoot<TId>, IHasVersion
    where TId : notnull
{
    /// <inheritdoc />
    public long Version { get; private set; }

    /// <summary>
    /// Initializes a new instance of <see cref="VersionedAggregateRoot{TId}"/>.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    protected VersionedAggregateRoot(TId id) : base(id)
    {
    }

    /// <inheritdoc />
    public void IncrementVersion()
    {
        checked
        {
            Version++;
        }
    }
}
