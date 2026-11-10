# MultiTenancy

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.MultiTenancy`

## What it is
`ITenantScoped` defines a contract for entities and models that are scoped to a single tenant in multi-tenant architectures.

## When to use it
- Use it on all domain entities in systems requiring logical tenant data isolation.
- Use it to enforce tenant boundaries via infrastructure query filters and save interceptors.
- Do NOT populate `AssignTenant` manually in domain logic; the multi-tenancy infrastructure middleware or interceptor populates this based on the current tenant context.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Implement `ITenantScoped` on entities requiring tenant isolation.

## Sample code
```csharp
using System;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.MultiTenancy;

namespace Example;

public sealed class TenantInvoice : Entity<Guid>, ITenantScoped
{
    public TenantInvoice(Guid id, decimal amount) : base(id)
    {
        Amount = amount;
    }

    private TenantInvoice() : base() { }

    public decimal Amount { get; private set; }
    public string TenantId { get; private set; } = string.Empty;

    public void AssignTenant(string tenantId)
    {
        TenantId = tenantId;
    }
}
```

## Configuration (if any)
Integrated with `SystemX.Infrastructure.MultiTenancy` to resolve the active tenant ID from HTTP requests or message headers.

## Related
- [Entity.md](Entity.md)
- [Auditing.md](Auditing.md)
