# Entity

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Domain`

## What it is
`Entity<TId>` is an abstract base class representing Domain-Driven Design entities that possess a distinct identity (`TId`).

## When to use it
- Use it when modeling domain objects distinguished by identity rather than their attributes.
- Use it when you need identity-based equality comparison.
- Do NOT use it when modeling concepts that are identified purely by their values; use `ValueObject` instead.
- Do NOT use it as the aggregate boundary; use `AggregateRoot<TId>` for aggregate root boundaries.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Inherit from `Entity<TId>`, providing the strongly-typed or primitive identifier type. Use `CheckRule` to enforce invariants within entity methods.

## Sample code
```csharp
using System;
using SystemX.SharedKernel.Domain;

namespace Example;

public sealed class OrderItem : Entity<Guid>
{
    public OrderItem(Guid id, string productName, decimal unitPrice, int quantity)
        : base(id)
    {
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    private OrderItem() : base() { }

    public string ProductName { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
}
```

## Configuration (if any)
No external configuration required.

## Related
- [AggregateRoot.md](AggregateRoot.md)
- [ValueObject.md](ValueObject.md)
- [BusinessRules.md](BusinessRules.md)
