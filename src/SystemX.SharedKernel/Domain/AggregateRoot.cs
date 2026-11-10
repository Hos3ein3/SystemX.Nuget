using SystemX.SharedKernel.Guards;

namespace SystemX.SharedKernel.Domain;

/// <summary>Base class for aggregate roots. It collects domain events.</summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Creates an aggregate root with an id.</summary>
    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>Parameterless constructor for ORMs only.</summary>
    protected AggregateRoot()
    {
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <inheritdoc />
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>Adds a domain event to be dispatched after the unit of work is saved.</summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(Guard.NotNull(domainEvent));
}
