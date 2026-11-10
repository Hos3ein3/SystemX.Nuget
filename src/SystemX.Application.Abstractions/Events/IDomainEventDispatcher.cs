using SystemX.SharedKernel.Domain;

namespace SystemX.Application.Abstractions.Events;

/// <summary>
/// Dispatches raised domain events to their respective application event handlers.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches a sequence of domain events asynchronously.
    /// </summary>
    /// <param name="domainEvents">The domain events to dispatch.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
