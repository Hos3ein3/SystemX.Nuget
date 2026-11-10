# SystemXDbContext

**Package:** `SystemX.Infrastructure.Persistence.EfCore`
**Namespace:** `SystemX.Infrastructure.Persistence.EfCore.Context`

## What it is
The abstract base `DbContext` that integrates SystemX multi-tenancy models, automated auditing, soft deletion, optimistic concurrency tokens, global query filters, and asynchronous domain event dispatching.

## When to use it
- Inherit from `SystemXDbContext` in your application's Infrastructure/Persistence project as your core application DbContext.
- Do NOT use plain `Microsoft.EntityFrameworkCore.DbContext` directly if you need multi-tenancy, domain events, or SystemX entity markers.

## Installation
`dotnet add package SystemX.Infrastructure.Persistence.EfCore`

## How to use it
1. Create your context class deriving from `SystemXDbContext`.
2. Provide a constructor accepting `DbContextOptions<T>` and `SystemXDbContextDependencies<T>`.
3. Override `ConfigureModel(ModelBuilder)` to configure your entity mappings or call `modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly)`.
4. Register the context with `services.AddSystemXDbContext<AppDbContext>(options => ...)`.

## Architectural Decisions
- **`OnModelCreating` is Sealed:** SystemX seals `OnModelCreating` to strictly ensure the correct execution order of pipeline setup: first calling `ConfigureModel`, then applying entity markers (concurrency, audit, tenant indices), followed by tenancy schema/table mappings, and finally query filters.
- **Synchronous `SaveChanges` is Not Supported:** Calling synchronous `SaveChanges()` throws `NotSupportedException`. Domain event handlers and dispatchers are inherently asynchronous (`Task DispatchAsync(...)`). To prevent sync-over-async deadlocks, only `SaveChangesAsync` is supported.

## Sample code

### Derived DbContext Implementation

```csharp
using Microsoft.EntityFrameworkCore;
using SystemX.Infrastructure.Persistence.EfCore.Context;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.MultiTenancy;

namespace InfrastructureSample;

public sealed record MemberId(Guid Value) : GuidId(Value), IGuidIdFactory<MemberId>
{
    public static MemberId Create(Guid value) => new(value);
}

public class Member : AggregateRoot<MemberId>, ITenantScoped
{
    public Member(MemberId id, string email) : base(id) => Email = email;
    public string Email { get; private set; }
    public TenantId TenantId { get; private set; } = null!;
    public void AssignTenant(TenantId tenantId) => TenantId = tenantId;
}

public sealed class AppDbContext : SystemXDbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        SystemXDbContextDependencies<AppDbContext> dependencies)
        : base(options, dependencies)
    {
    }

    public DbSet<Member> Members => Set<Member>();

    protected override void ConfigureModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>(b =>
        {
            b.HasKey(m => m.Id);
            b.Property(m => m.Email).HasMaxLength(256).IsRequired();
        });
    }
}
```

### Registration in Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InfrastructureSample;

namespace InfrastructureSample;

public static class StartupExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        return services.AddSystemXDbContext<AppDbContext>(options =>
        {
            options.HostConnectionString = connectionString;
            options.ConfigureProvider = (builder, cs) => builder.UseSqlServer(cs);
            options.MaxDomainEventDepth = 5;
        });
    }
}
```

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `HostConnectionString` | `null` | Connection string of the host/shared database. |
| `ConfigureProvider` | `null` | Action configuring the provider (e.g., UseSqlServer, UseNpgsql). Required. |
| `MaxDomainEventDepth` | `5` | Maximum recursive dispatch depth for domain events. |
| `IgnoreTenancy` | `false` | When true, ignores tenant filters completely. |
| `RequireTenant` | `true` | When true, missing tenant blocks tenant-scoped data. |

## Related
- [SavePipeline](SavePipeline.md)
- [QueryFilters](QueryFilters.md)
- [TenancyModels](TenancyModels.md)
- [Concurrency](Concurrency.md)
