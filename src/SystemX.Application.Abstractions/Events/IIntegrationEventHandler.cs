namespace SystemX.Application.Abstractions.Events;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Handles a received integration event of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of the integration event.</typeparam>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Standard integration event handler naming convention.")]
public interface IIntegrationEventHandler<in TEvent> where TEvent : class
{
    /// <summary>
    /// Handles the received integration event asynchronously.
    /// </summary>
    /// <param name="integrationEvent">The integration event instance.</param>
    /// <param name="context">The transport metadata and context for the event.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous handling operation.</returns>
    Task HandleAsync(TEvent integrationEvent, IntegrationEventContext context, CancellationToken cancellationToken = default);
}
