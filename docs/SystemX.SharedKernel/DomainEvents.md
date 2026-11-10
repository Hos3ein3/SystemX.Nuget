# Domain Events

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Domain`

## What it is
`IDomainEvent` and `DomainEvent` provide standard contracts and base records for capturing significant domain occurrences within an aggregate.

## When to use it
- Use it to notify other components or side effects about state transitions inside an aggregate.
- Use `RaiseDomainEvent` on `AggregateRoot<TId>` to stage events.
- Do NOT dispatch domain events immediately in aggregate methods; collect them and allow infrastructure (e.g., an EF Core `DbContext` SaveChanges interceptor) to dispatch them after the transaction successfully commits.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Create domain events deriving from `DomainEvent`. Aggregate roots collect them via `RaiseDomainEvent` and expose them via `DomainEvents`. Infrastructure code reads them and clears them using `ClearDomainEvents()`.

## Sample code
```csharp
using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Domain;

namespace Example;

public sealed record OrderPaidDomainEvent(Guid OrderId, DateTimeOffset PaidAt) : DomainEvent;

public sealed class OrderWithEvents : AggregateRoot<Guid>
{
    public OrderWithEvents(Guid id) : base(id) { }
    private OrderWithEvents() : base() { }

    public bool IsPaid { get; private set; }

    public void MarkAsPaid()
    {
        IsPaid = true;
        RaiseDomainEvent(new OrderPaidDomainEvent(Id, DateTimeOffset.UtcNow));
    }
}

public static class DomainEventDispatcherExample
{
    public static void DispatchAndClear(IHasDomainEvents entity)
    {
        foreach (var @event in entity.DomainEvents)
        {
            // Dispatch via mediator or event bus
            Console.WriteLine($"Dispatched event: {@event.EventId} at {@event.OccurredOn}");
        }
        entity.ClearDomainEvents();
    }
}
```

## Configuration (if any)
No external configuration required.

## Related
- [AggregateRoot.md](AggregateRoot.md)
