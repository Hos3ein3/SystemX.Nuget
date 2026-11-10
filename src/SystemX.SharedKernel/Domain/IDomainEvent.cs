namespace SystemX.SharedKernel.Domain;

/// <summary>Something important that happened in the domain.</summary>
public interface IDomainEvent
{
    /// <summary>Gets the unique id of this event instance.</summary>
    Guid EventId { get; }

    /// <summary>Gets the UTC time when the event occurred.</summary>
    DateTimeOffset OccurredOn { get; }
}
