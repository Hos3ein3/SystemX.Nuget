# BusinessRules

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Domain`

## What it is
`IBusinessRule`, `BusinessRuleValidationException`, and `DomainException` provide a structured model for defining and enforcing business invariants within domain entities and aggregates.

## When to use it
- Use it to encapsulate domain validation rules that depend on entity state or multiple properties.
- Use it inside entity or aggregate methods via `CheckRule(new MyCustomRule(...))`.
- Do NOT use it for trivial technical input checks like non-null argument checking; use `Guard` instead.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Implement `IBusinessRule` with a stable error `Code`, a human-readable `Message`, and the `IsBroken()` predicate. Call `CheckRule(rule)` within entities.

## Sample code
```csharp
using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Domain;

namespace Example;

public sealed class OrderMustHaveLinesRule : IBusinessRule
{
    private readonly int _lineCount;

    public OrderMustHaveLinesRule(int lineCount)
    {
        _lineCount = lineCount;
    }

    public string Code => "Order.MustHaveLines";
    public string Message => "An order must contain at least one order line.";

    public bool IsBroken() => _lineCount <= 0;
}

public sealed class ValidatedOrder : Entity<Guid>
{
    private readonly List<string> _items = [];

    public ValidatedOrder(Guid id) : base(id) { }

    public void Complete()
    {
        CheckRule(new OrderMustHaveLinesRule(_items.Count));
    }
}
```

## Configuration (if any)
No external configuration required.

## Related
- [Entity.md](Entity.md)
- [AggregateRoot.md](AggregateRoot.md)
