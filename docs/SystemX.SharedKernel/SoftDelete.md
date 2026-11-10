# SoftDelete

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Auditing`

## What it is
`ISoftDeletable` defines a contract for entities that should be marked as deleted rather than permanently removed from the data store.

## When to use it
- Use it for entities whose historical data must be preserved for auditing, compliance, or recovery.
- Use it in conjunction with global query filters in data access layers.
- Do NOT use it for temporary or non-critical transactional data where hard deletion is required.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Implement `ISoftDeletable` on entities. Infrastructure layers (e.g., EF Core interceptors) automatically invoke `MarkDeleted` when an entity is removed from a repository.

## Sample code
```csharp
using System;
using SystemX.SharedKernel.Auditing;
using SystemX.SharedKernel.Domain;

namespace Example;

public sealed class SoftDeletableProduct : Entity<Guid>, ISoftDeletable
{
    public SoftDeletableProduct(Guid id, string title) : base(id)
    {
        Title = title;
    }

    private SoftDeletableProduct() : base() { }

    public string Title { get; private set; } = null!;
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public void MarkDeleted(DateTimeOffset at, string? by)
    {
        IsDeleted = true;
        DeletedAt = at;
        DeletedBy = by;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
```

## Configuration (if any)
Global query filters can be configured in EF Core persistence to automatically filter out soft-deleted entities.

## Related
- [Auditing.md](Auditing.md)
- [Entity.md](Entity.md)
