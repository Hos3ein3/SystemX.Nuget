namespace SystemX.SharedKernel.Domain;

/// <summary>Marker for aggregate roots. Only aggregate roots are loaded and saved by repositories.</summary>
public interface IAggregateRoot : IHasDomainEvents
{
}
