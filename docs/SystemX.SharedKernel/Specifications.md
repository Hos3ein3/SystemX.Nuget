# Specifications

**Package:** `SystemX.SharedKernel`  
**Namespace:** `SystemX.SharedKernel.Specifications`

## What it is
A pure domain-level query pattern encapsulating filtering criteria, ordering, navigation includes, and pagination into reusable, composable objects independent of any database provider.

## When to use it
- Use it to encapsulate domain query rules and logic instead of writing ad-hoc LINQ queries across services.
- Use it to test query criteria in memory using `SpecificationExtensions.Evaluate`.
- Do NOT use it for complex aggregate write boundaries or raw native SQL optimization queries.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
1. Inherit from `Specification<T>` or `Specification<T, TResult>`.
2. Configure criteria in the constructor via `Where`, `OrderBy`, `OrderByDescending`, `Include`, and `ApplyPaging`.
3. Compose specifications using the logical operators `&`, `|`, and `!`.

> [!NOTE]
> Database query evaluation (translating specifications into SQL via EF Core `IQueryable`) is implemented in `SystemX.Infrastructure.Persistence.EfCore`.

## Sample code

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.Pagination;
using SystemX.SharedKernel.Specifications;

namespace Example.SpecificationsDoc;

public sealed record CustomerId(Guid Value) : GuidId(Value), IGuidIdFactory<CustomerId>
{
    public static CustomerId Create(Guid value) => new(value);
}

public sealed record OrderId(Guid Value) : GuidId(Value), IGuidIdFactory<OrderId>
{
    public static OrderId Create(Guid value) => new(value);
}

public sealed class Order : Entity<OrderId>
{
    public CustomerId CustomerId { get; }
    public decimal TotalAmount { get; }
    public bool IsActive { get; }

    public Order(OrderId id, CustomerId customerId, decimal totalAmount, bool isActive) : base(id)
    {
        CustomerId = customerId;
        TotalAmount = totalAmount;
        IsActive = isActive;
    }
}

public sealed class ActiveOrdersByCustomerSpec : Specification<Order>
{
    public ActiveOrdersByCustomerSpec(CustomerId customerId, decimal minAmount)
    {
        Where(o => o.CustomerId == customerId && o.IsActive);
        Where(o => o.TotalAmount >= minAmount);
        OrderByDescending(o => o.TotalAmount);
    }
}

public static class SpecificationSample
{
    public static void Run()
    {
        CustomerId customerId = CustomerId.Create(Guid.NewGuid());
        List<Order> orders =
        [
            new Order(OrderId.Create(Guid.NewGuid()), customerId, 150m, true),
            new Order(OrderId.Create(Guid.NewGuid()), customerId, 50m, false),
            new Order(OrderId.Create(Guid.NewGuid()), customerId, 250m, true)
        ];

        ActiveOrdersByCustomerSpec spec = new(customerId, 100m);
        List<Order> filtered = orders.Evaluate(spec).ToList();
        Console.WriteLine($"Found {filtered.Count} active high-value orders.");
    }
}
```

## Configuration (if any)
| Setting | Default | Description |
|---|---|---|
| `AsNoTracking` | `true` | Defaults to non-tracking queries for read performance. |
| `AsSplitQuery` | `false` | Whether to split collection includes across queries. |
| `IgnoreQueryFilters` | `false` | Whether global query filters (soft delete, tenant) are bypassed. |

## Related
- [StronglyTypedIds](StronglyTypedIds.md)
- [Pagination](Pagination.md)
