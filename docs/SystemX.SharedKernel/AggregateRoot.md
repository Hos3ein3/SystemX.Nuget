# AggregateRoot

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Domain`

## What it is
`AggregateRoot<TId>` is an abstract base class for aggregate roots in Domain-Driven Design that encapsulates consistency boundaries and collects domain events.

## When to use it
- Use it to define the root entity of an aggregate boundary.
- Use it when you need to record and raise domain events during business operations.
- Do NOT use it for child entities within the aggregate; inherit from `Entity<TId>` instead.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Inherit from `AggregateRoot<TId>`. Invoke `RaiseDomainEvent` whenever an operation produces a state change that other parts of the system or external bounded contexts must react to.

## Sample code
```csharp
using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Domain;

namespace Example;

public sealed class OrderLine : Entity<Guid>
{
    public OrderLine(Guid id, string productName, decimal unitPrice, int quantity) : base(id)
    {
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    private OrderLine() : base() { }

    public string ProductName { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
}

public sealed record OrderPlacedDomainEvent(Guid OrderId, decimal TotalAmount) : DomainEvent;

public sealed class Order : AggregateRoot<Guid>
{
    private readonly List<OrderLine> _items = [];

    public Order(Guid id, Guid customerId) : base(id)
    {
        CustomerId = customerId;
    }

    private Order() : base() { }

    public Guid CustomerId { get; private set; }
    public IReadOnlyCollection<OrderLine> Items => _items.AsReadOnly();

    public void AddItem(string productName, decimal unitPrice, int quantity)
    {
        _items.Add(new OrderLine(Guid.NewGuid(), productName, unitPrice, quantity));
        RaiseDomainEvent(new OrderPlacedDomainEvent(Id, unitPrice * quantity));
    }
}
```

## Configuration (if any)
No external configuration required.

## Related
- [Entity.md](Entity.md)
- [DomainEvents.md](DomainEvents.md)
