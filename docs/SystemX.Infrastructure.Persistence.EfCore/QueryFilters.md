# QueryFilters

**Package:** `SystemX.Infrastructure.Persistence.EfCore`
**Namespace:** `SystemX.Infrastructure.Persistence.EfCore.Context`

## What it is
Global query filters applied automatically to entity types in `SystemXDbContext` to isolate tenant data, conceal soft-deleted records, and filter out archived entities.

## When to use it
- Filters are active by default across all repository and LINQ queries.
- Use `context.DisableFilter(DataFilter)` to temporarily bypass filters within a `using` scope (e.g., in administrative screens or data restore operations).
- Do NOT disable `DataFilter.Tenant` without strict authorization; doing so exposes cross-tenant data.

## Why `FindAsync` is Never Used
In `EfReadRepository` and `EfRepository`, `GetByIdAsync` deliberately executes `Set.FirstOrDefaultAsync(IdPredicate.Build<TEntity, TId>(id))` instead of EF Core's built-in `DbSet.FindAsync`. In EF Core, `FindAsync` inspects local change tracker state by primary key before consulting the database, **bypassing global query filters**. Using `FindAsync` would risk leaking deleted or cross-tenant records already held in memory.

## Sample code

### Bypassing Soft Delete to View and Restore an Entity

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SystemX.Application.Abstractions.Persistence;
using SystemX.Infrastructure.Persistence.EfCore.Context;
using SystemX.SharedKernel.Auditing;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;

namespace InfrastructureSample;

public sealed record PostId(Guid Value) : GuidId(Value), IGuidIdFactory<PostId>
{
    public static PostId Create(Guid value) => new(value);
}

public class Post : AggregateRoot<PostId>, ISoftDeletable
{
    public Post(PostId id, string title) : base(id) => Title = title;
    public string Title { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }
    public void MarkDeleted(DateTimeOffset at, string? by) { IsDeleted = true; DeletedAt = at; DeletedBy = by; }
    public void Restore() { IsDeleted = false; DeletedAt = null; DeletedBy = null; }
}

public sealed class PostRestorationService
{
    private readonly SystemXDbContext _context;

    public PostRestorationService(SystemXDbContext context)
    {
        _context = context;
    }

    public async Task RestorePostAsync(PostId postId, CancellationToken cancellationToken)
    {
        // Disable soft-delete filter temporarily to load the deleted post
        using (_context.DisableFilter(DataFilter.SoftDelete))
        {
            var post = await _context.Set<Post>().FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
            if (post is not null && post.IsDeleted)
            {
                post.Restore();
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
```

### The Danger of Disabling the Tenant Filter

```csharp
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SystemX.Infrastructure.Persistence.EfCore.Context;

namespace InfrastructureSample;

public sealed class PlatformAuditor
{
    private readonly SystemXDbContext _context;

    public PlatformAuditor(SystemXDbContext context) => _context = context;

    public async Task<int> CountAllPostsAcrossAllTenantsAsync(CancellationToken cancellationToken)
    {
        // CAUTION: Disabling DataFilter.Tenant exposes every tenant's data across the entire database!
        // This must be restricted to super-administrator background workers and audit jobs.
        using (_context.DisableFilter(DataFilter.Tenant))
        {
            return await _context.Set<Post>().CountAsync(cancellationToken);
        }
    }
}
```

## Configuration (if any)
| Filter Flag | Enum Value | Description |
|---|---|---|
| `DataFilter.SoftDelete` | `0` | Conceals entities where `IsDeleted == true`. |
| `DataFilter.Archived` | `1` | Conceals entities where `IsArchived == true`. |
| `DataFilter.Tenant` | `2` | Enforces `TenantId == CurrentTenantId` for Row multi-tenancy. |

## Related
- [SystemXDbContext](SystemXDbContext.md)
- [TenancyModels](TenancyModels.md)
- [Repositories](Repositories.md)
