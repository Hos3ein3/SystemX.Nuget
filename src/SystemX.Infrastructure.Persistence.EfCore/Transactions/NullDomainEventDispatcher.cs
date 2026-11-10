using SystemX.Application.Abstractions.Events;
using SystemX.SharedKernel.Domain;

namespace SystemX.Infrastructure.Persistence.EfCore.Transactions;

/// <summary>
/// Default no-op domain event dispatcher used when no application event dispatcher is registered.
/// </summary>
public sealed class NullDomainEventDispatcher : IDomainEventDispatcher
{
    /// <summary>
    /// Gets the singleton instance of <see cref="NullDomainEventDispatcher"/>.
    /// </summary>
    public static readonly NullDomainEventDispatcher Instance = new();

    /// <inheritdoc />
    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
