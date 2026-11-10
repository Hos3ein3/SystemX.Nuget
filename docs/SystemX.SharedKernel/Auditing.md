# Auditing

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Auditing`

## What it is
`IAuditable`, `AuditableEntity<TId>`, and `AuditableAggregateRoot<TId>` define audit metadata contracts and base types for tracking creation and modification timestamps and authors.

## When to use it
- Use it on entities or aggregate roots that require change history tracking.
- Use it to let persistence interceptors populate audit trails automatically.
- Do NOT populate `SetCreated` or `SetModified` manually in domain logic; an EF Core SaveChanges interceptor in the persistence infrastructure sets these automatically.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Derive domain entities from `AuditableEntity<TId>` or aggregate roots from `AuditableAggregateRoot<TId>`.

## Sample code
```csharp
using System;
using SystemX.SharedKernel.Auditing;

namespace Example;

public sealed class AuditedCustomer : AuditableAggregateRoot<Guid>
{
    public AuditedCustomer(Guid id, string name) : base(id)
    {
        Name = name;
    }

    private AuditedCustomer() : base() { }

    public string Name { get; private set; } = null!;

    public void Rename(string newName)
    {
        Name = newName;
    }
}
```

## Configuration (if any)
Audit tracking is automatically populated when using the `SystemX.Infrastructure.Persistence.EfCore` interceptor.

## Related
- [Entity.md](Entity.md)
- [AggregateRoot.md](AggregateRoot.md)
- [SoftDelete.md](SoftDelete.md)
