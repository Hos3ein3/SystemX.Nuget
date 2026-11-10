using SystemX.SharedKernel.Domain;

namespace SystemX.SharedKernel.Concurrency;

/// <summary>
/// Abstract base aggregate root implementing stamp-based optimistic concurrency tracking (<see cref="IHasConcurrencyStamp"/>).
/// </summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public abstract class StampedAggregateRoot<TId> : AggregateRoot<TId>, IHasConcurrencyStamp
    where TId : notnull
{
    /// <inheritdoc />
    public string ConcurrencyStamp { get; private set; }

    /// <summary>
    /// Initializes a new instance of <see cref="StampedAggregateRoot{TId}"/> with an initial random stamp.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    protected StampedAggregateRoot(TId id) : base(id)
    {
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }

    /// <inheritdoc />
    public void RenewConcurrencyStamp()
    {
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }
}
