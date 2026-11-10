# Tenant Entity Markers

**Package:** `SystemX.SharedKernel`  
**Namespace:** `SystemX.SharedKernel.MultiTenancy`

## What it is
Domain-level entity marker interfaces that declare how records associate with tenants, guiding persistence interceptors and query filters.

## When to use it
- Use `ITenantScoped` when an entity strictly belongs to exactly one tenant.
- Use `IMayHaveTenant` for shared lookup data or system templates that individual tenants can override.
- Use `ISharedEntity` for static system-wide reference data (e.g. currency tables, country codes) that is never filtered by tenant.
- Do NOT implement more than one of `ITenantScoped`, `IMayHaveTenant`, or `ISharedEntity` on the same class.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Implement the appropriate marker on your entity or aggregate root.

## Sample code

```csharp
using System;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.MultiTenancy;

namespace Example.TenantMarkersDoc;

public sealed record OrderId(Guid Value) : GuidId(Value), IGuidIdFactory<OrderId>
{
    public static OrderId Create(Guid value) => new(value);
}

// Tenant-scoped entity
public sealed class Order : AggregateRoot<OrderId>, ITenantScoped
{
    public TenantId TenantId { get; private set; }
    public decimal TotalAmount { get; }

    public Order(OrderId id, TenantId tenantId, decimal totalAmount) : base(id)
    {
        TenantId = tenantId;
        TotalAmount = totalAmount;
    }

    public void AssignTenant(TenantId tenantId)
    {
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
    }
}

// Shared reference data
public sealed class Country : Entity<string>, ISharedEntity
{
    public string Name { get; }

    public Country(string code, string name) : base(code)
    {
        Name = name;
    }
}

public static class MarkerUsageSample
{
    public static void Run()
    {
        Order order = new(OrderId.Create(Guid.NewGuid()), TenantId.Create("tenant-1"), 250m);
        Country country = new("US", "United States");

        Console.WriteLine($"Order tenant: {order.TenantId.Value}");
        Console.WriteLine($"Country: {country.Name}");
    }
}
```

## Related
- [MultiTenancyModels](MultiTenancyModels.md)
- [EntityMarkers](EntityMarkers.md)
