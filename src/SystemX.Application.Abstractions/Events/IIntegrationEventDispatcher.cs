namespace SystemX.Application.Abstractions.Events;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Routes a RECEIVED integration event (from a broker, the inbox or an in-process source) to its handlers.
/// Transport consumers call it. It is not used to publish; publishing is IEventBus / IOutbox.
/// </summary>
public interface IIntegrationEventDispatcher
{
    /// <summary>
    /// Dispatches a received integration event to all registered handlers for <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the integration event.</typeparam>
    /// <param name="integrationEvent">The integration event payload.</param>
    /// <param name="context">The transport metadata and context for the received event.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous dispatch operation.</returns>
    Task DispatchAsync<TEvent>(TEvent integrationEvent, IntegrationEventContext context, CancellationToken cancellationToken = default)
        where TEvent : class;

    /// <summary>
    /// Dispatches using the runtime type of the event.
    /// </summary>
    /// <param name="integrationEvent">The integration event payload instance.</param>
    /// <param name="context">The transport metadata and context for the received event.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous dispatch operation.</returns>
    Task DispatchAsync(object integrationEvent, IntegrationEventContext context, CancellationToken cancellationToken = default);
}
