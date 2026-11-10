# SavePipeline

**Package:** `SystemX.Infrastructure.Persistence.EfCore`
**Namespace:** `SystemX.Infrastructure.Persistence.EfCore.Context`

## What it is
The automated interceptor and dispatch pipeline executed whenever `SaveChangesAsync` is invoked on `SystemXDbContext`.

## When to use it
- The pipeline executes transparently upon calling `IUnitOfWork.SaveChangesAsync`.
- Do NOT manually assign audit timestamps, tenant IDs, or concurrency stamps in command handlers; let the save pipeline handle them automatically.

## Pipeline Execution Order
When `SaveChangesAsync` is called, the pipeline executes in the following sequence:

1. **`ChangeTracker.DetectChanges()`:** Identifies all new, modified, or deleted entity entries.
2. **`ApplySaveConventions()`:** Evaluates entity markers in the following strict order:
   - **Multi-Tenancy (`ITenantScoped`, `IMayHaveTenant`):** Assigns `CurrentTenantId` to new entities. Throws `TenantNotResolvedException` if inserting tenant-scoped data without an active tenant, or `InvalidOperationException` if the tenant ID is altered or mismatched.
   - **Creation Auditing (`IHasCreationTime`, `IAuditable`):** Sets `CreatedAt` and `CreatedBy` from `IClock.UtcNow` and `ICurrentUser.UserId`.
   - **Immutability (`IImmutableEntity`):** Throws `InvalidOperationException` if an immutable entity is modified or deleted.
   - **Modification Auditing (`IHasModificationTime`, `IAuditable`):** Sets `ModifiedAt` and `ModifiedBy` on modified entities.
   - **Concurrency Tokens (`IHasConcurrencyStamp`, `IHasVersion`):** Renews `ConcurrencyStamp` (GUID) or increments `Version` (numeric).
   - **Soft Delete (`ISoftDeletable`):** Changes `EntityState.Deleted` into `EntityState.Modified`, setting `IsDeleted = true`, `DeletedAt`, and `DeletedBy`.
3. **Domain Event Collection:** Extracts all pending events from entities implementing `IHasDomainEvents`.
4. **Database Commit (`SaveCoreAsync`):** Persists all pending change tracker entries to the database inside the current transaction. Maps `DbUpdateConcurrencyException` to `ConcurrencyException`.
5. **Clear Domain Events:** Clears events from tracked entities.
6. **Domain Event Dispatch Loop:**
   - Dispatches collected events via `IDomainEventDispatcher.DispatchAsync`.
   - If event handlers mutated any entities, re-runs change detection, conventions, and saves additional changes.
   - Repeats until no pending events remain, up to `MaxDomainEventDepth` (default 5), throwing `InvalidOperationException` if an infinite loop is detected.

## Outbox Pattern and Integration Events
Domain events are dispatched immediately after database changes are written, participating in the same transaction scope. However, external communication (e.g., publishing to RabbitMQ, Azure Service Bus, or Kafka) must NOT be done directly inside domain event handlers, because network failure after DB commit would cause lost messages. Use the SystemX Outbox pattern (`SystemX.Infrastructure.Messaging`) to guarantee at-least-once message delivery for external integration events.

## Sample code

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.Auditing;
using SystemX.SharedKernel.Concurrency;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.MultiTenancy;

namespace ApplicationSample;

public sealed record AccountId(Guid Value) : GuidId(Value), IGuidIdFactory<AccountId>
{
    public static AccountId Create(Guid value) => new(value);
}

public class Account : AggregateRoot<AccountId>, ITenantScoped, IAuditable, IHasConcurrencyStamp
{
    public Account(AccountId id, string name) : base(id) => Name = name;

    public string Name { get; private set; }
    public TenantId TenantId { get; private set; } = null!;
    public void AssignTenant(TenantId tenantId) => TenantId = tenantId;

    public DateTimeOffset CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTimeOffset? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public void SetCreated(DateTimeOffset at, string? by) { CreatedAt = at; CreatedBy = by; }
    public void SetModified(DateTimeOffset at, string? by) { ModifiedAt = at; ModifiedBy = by; }

    public string ConcurrencyStamp { get; private set; } = Guid.NewGuid().ToString("N");
    public void RenewConcurrencyStamp() => ConcurrencyStamp = Guid.NewGuid().ToString("N");
}

public sealed class CreateAccountHandler
{
    private readonly IRepository<Account, AccountId> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountHandler(IRepository<Account, AccountId> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(AccountId id, string name, CancellationToken cancellationToken)
    {
        var account = new Account(id, name);
        _repository.Add(account);

        // Save pipeline automatically populates TenantId, CreatedAt, CreatedBy, and ConcurrencyStamp
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `MaxDomainEventDepth` | `5` | Limits recursive event dispatch rounds to prevent infinite loops. |

## Related
- [SystemXDbContext](SystemXDbContext.md)
- [Concurrency](Concurrency.md)
- [QueryFilters](QueryFilters.md)
- [TenancyModels](TenancyModels.md)
