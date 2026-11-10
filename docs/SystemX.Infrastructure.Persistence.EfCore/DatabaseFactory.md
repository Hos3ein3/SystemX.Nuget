# DatabaseFactory

**Package:** `SystemX.Infrastructure.Persistence.EfCore`
**Namespace:** `SystemX.Infrastructure.Persistence.EfCore.Sessions`

## What it is
Implementation of `IDatabaseFactory` that creates isolated database sessions (`IDatabaseSession`) with scoped DbContext instances for background jobs, cross-tenant loops, and host administration.

## When to use it
- Use it in background workers, scheduled jobs, or event consumers that need to process data for specific tenants or across all tenants.
- Use it to run host-level administrative migrations or maintenance tasks via `CreateHostSessionAsync()`.
- Do NOT use it for standard request/response controllers or endpoints where scoped DI services (`IUnitOfWork`, `IRepository<,>`) already provide request-isolated contexts.

## Installation
`dotnet add package SystemX.Infrastructure.Persistence.EfCore`

## How to use it
Register the DbContext and factories using `AddSystemXDbContext<TContext>`:

```csharp
services.AddSystemXDbContext<AppDbContext>(options =>
{
    options.HostConnectionString = "Server=localhost;Database=HostDb;Integrated Security=true;";
    options.ConfigureProvider = (builder, cs) => builder.UseSqlServer(cs);
});
```

Inject `IDatabaseFactory` into your service and call `CreateSessionForTenantAsync` or `CreateHostSessionAsync`. Ensure sessions are disposed using `await using`.

## Sample code

### Background Job Looping Over All Tenants

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.MultiTenancy;

namespace ApplicationSample;

public sealed record InvoiceId(Guid Value) : GuidId(Value), IGuidIdFactory<InvoiceId>
{
    public static InvoiceId Create(Guid value) => new(value);
}

public class Invoice : AggregateRoot<InvoiceId>, ITenantScoped
{
    public Invoice(InvoiceId id, decimal total) : base(id) => Total = total;
    public decimal Total { get; private set; }
    public TenantId TenantId { get; private set; } = null!;
    public void AssignTenant(TenantId tenantId) => TenantId = tenantId;
}

public sealed class MonthlyBillingJob
{
    private readonly ITenantStore _tenantStore;
    private readonly IDatabaseFactory _databaseFactory;

    public MonthlyBillingJob(ITenantStore tenantStore, IDatabaseFactory databaseFactory)
    {
        _tenantStore = tenantStore;
        _databaseFactory = databaseFactory;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var tenants = await _tenantStore.GetAllAsync(cancellationToken);

        foreach (var tenant in tenants)
        {
            if (!tenant.IsActive)
            {
                continue;
            }

            await using var session = await _databaseFactory.CreateSessionForTenantAsync(tenant, cancellationToken);
            var invoices = session.Repositories.GetRepository<Invoice, InvoiceId>();

            var invoice = new Invoice(InvoiceId.Create(Guid.NewGuid()), 99.00m);
            invoices.Add(invoice);

            await session.SaveChangesAsync(cancellationToken);
        }
    }
}
```

### Host Administration Session Sample

```csharp
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.Specifications;

namespace ApplicationSample;

public sealed class HostMaintenanceWorker
{
    private readonly IDatabaseFactory _databaseFactory;

    public HostMaintenanceWorker(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task PurgeAllTenantInvoicesAsync(CancellationToken cancellationToken)
    {
        // Host session runs with TenancyModel.None; tenant query filters are disabled.
        await using var hostSession = await _databaseFactory.CreateHostSessionAsync(cancellationToken);
        var invoices = hostSession.Repositories.GetRepository<Invoice, InvoiceId>();

        var allInvoices = await invoices.ListAsync(new AllInvoicesSpec(), cancellationToken);
        invoices.RemoveRange(allInvoices);

        await hostSession.SaveChangesAsync(cancellationToken);
    }

    private sealed class AllInvoicesSpec : Specification<Invoice> { }
}
```

## Architectural Limitations of Sessions
- **Isolated DI Scope:** Each `IDatabaseSession` owns its own `AsyncServiceScope` and private `SystemXDbContext` instance.
- **Domain Event Handling Boundary:** Domain events dispatched during `session.SaveChangesAsync` run inside the session's DI scope. Handlers resolved from DI will not share an ambient outer transaction. Aggregates requiring transactional coordination with ambient outer scopes must use the standard request-scope `IUnitOfWork`.
- **Target Use Cases:** Sessions are strictly intended for background jobs, asynchronous workers, and tenant provisioning. Primary request handlers should rely on standard constructor injection of repositories.

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `RegisterRepositories` | `true` | Enables registration of `IDatabaseFactory` and related interfaces. |

## Related
- [DatabaseFactories](../SystemX.Application.Abstractions/DatabaseFactories.md)
- [RepositoryFactory](RepositoryFactory.md)
- [ConnectionFactory](ConnectionFactory.md)
- [SystemXDbContext](SystemXDbContext.md)
