namespace SystemX.SharedKernel.Domain;

/// <summary>Represents an object that collects domain events.</summary>
public interface IHasDomainEvents
{
    /// <summary>Gets the events raised and not yet dispatched.</summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>Removes all pending events. Call it after dispatching.</summary>
    void ClearDomainEvents();
}
