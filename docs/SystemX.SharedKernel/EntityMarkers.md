# Entity Markers

**Package:** `SystemX.SharedKernel`  
**Namespace:** `SystemX.SharedKernel.Markers`

## What it is
Granular, single-responsibility marker interfaces signaling specific behaviors to persistence interceptors, query builders, and specifications.

## Summary of Markers

| Marker | Purpose | Infrastructure Behavior |
|---|---|---|
| `IHasCreationTime` | Records creation instant | Automatically set on entity insertion. |
| `IHasModificationTime` | Records last modification instant | Automatically updated on entity modification. |
| `IArchivable` | Soft archival state | Filtered out by default (`!IsArchived`). |
| `IActivatable` | Active / Inactive state toggle | Available to business specifications. |
| `IOrderable` | Explicit display ordering | Helper for default UI ordering. |
| `IImmutableEntity` | Immutable ledger / log record | Throws exception if update or delete is attempted. |
| `INotAudited` | Excluded from audit trail | Audit interceptors bypass logging. |
| `IReadModel` | CQRS read projection | Queried without tracking, never treated as aggregate. |
| `IOwnedBy<TOwnerId>` | Principal ownership | Enforces per-owner security filters. |

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Implement the required marker interface on your domain entity.

## Sample code

```csharp
using System;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.Markers;

namespace Example.EntityMarkersDoc;

public sealed record AuditLogId(Guid Value) : GuidId(Value), IGuidIdFactory<AuditLogId>
{
    public static AuditLogId Create(Guid value) => new(value);
}

public sealed class SecurityAuditEntry : Entity<AuditLogId>, IImmutableEntity, IHasCreationTime, INotAudited
{
    public string Action { get; }
    public DateTimeOffset CreatedAt { get; private set; }

    public SecurityAuditEntry(AuditLogId id, string action) : base(id)
    {
        Action = action;
    }

    public void SetCreationTime(DateTimeOffset createdAt)
    {
        CreatedAt = createdAt;
    }
}
```

## Related
- [TenantMarkers](TenantMarkers.md)
- [Concurrency](Concurrency.md)
