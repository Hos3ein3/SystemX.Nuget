# DatabaseFactories

**Package:** `SystemX.Application.Abstractions`
**Namespace:** `SystemX.Application.Abstractions.Persistence`

## What it is
Defines persistence factory abstractions (`IDatabaseFactory`, `IDatabaseSession`, `IRepositoryFactory`, and `IDatabaseConnectionFactory`) allowing application and background workflows to manage database sessions, repositories, and raw connections without referencing EF Core.

## When to use it
- Use `IDatabaseFactory` and `IDatabaseSession` when orchestrating work outside the ambient request scope, such as recurring background jobs, tenant batch processing, or host administrative maintenance.
- Use `IRepositoryFactory` when dynamic access to repositories is required across arbitrary aggregate roots sharing the same unit of work.
- Use `IDatabaseConnectionFactory` to open raw database connections for high-performance direct SQL or Dapper queries without coupling to EF Core.
- Do NOT use `IDatabaseFactory` in standard HTTP request handlers; prefer constructor injection of `IRepository<TEntity, TId>` and `IUnitOfWork` bound to the request scope.

## Installation
`dotnet add package SystemX.Application.Abstractions`

## How to use it
Inject `IDatabaseFactory` or `IDatabaseConnectionFactory` into your application service or background worker:

1. Request an isolated session via `IDatabaseFactory.CreateSessionForTenantAsync(tenant)` or `CreateHostSessionAsync()`.
2. Retrieve repositories through `session.Repositories.GetRepository<TEntity, TId>()`.
3. Save state mutations using `await session.SaveChangesAsync(cancellationToken)`.
4. Asynchronously dispose the session with `await using`.

## Sample code
The following sample demonstrates using `IDatabaseFactory` within a tenant-loop background service:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.MultiTenancy;

namespace ApplicationSample;

public sealed record OrderId(Guid Value) : GuidId(Value), IGuidIdFactory<OrderId>
{
    public static OrderId Create(Guid value) => new(value);
}

public class Order : AggregateRoot<OrderId>
{
    public Order(OrderId id, decimal amount) : base(id) => Amount = amount;
    public decimal Amount { get; private set; }
}

public sealed class NightlyOrderProcessor
{
    private readonly IDatabaseFactory _databaseFactory;

    public NightlyOrderProcessor(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task ProcessTenantOrdersAsync(TenantInfo tenant, CancellationToken cancellationToken)
    {
        await using var session = await _databaseFactory.CreateSessionForTenantAsync(tenant, cancellationToken);
        var orderRepository = session.Repositories.GetRepository<Order, OrderId>();

        var newOrder = new Order(OrderId.Create(Guid.NewGuid()), 150.00m);
        orderRepository.Add(newOrder);

        await session.SaveChangesAsync(cancellationToken);
    }
}
```

The second sample demonstrates opening a raw database connection using `IDatabaseConnectionFactory`:

```csharp
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.MultiTenancy;

namespace ApplicationSample;

public sealed class ReportingService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public ReportingService(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> GetActiveCustomerCountAsync(TenantInfo tenant, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.OpenConnectionAsync(tenant, "Default", cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM Customers";

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }
}
```

## Configuration (if any)
| Option / Parameter | Default | Description |
|---|---|---|
| `name` | `"Default"` | The identifier of the connection string to resolve. |
| `tenant` | `null` | Target tenant metadata. When `null`, ambient tenant context or host connection is used. |

## Related
- [ProtectingTheApplicationLayer](../SystemX.Infrastructure.Persistence.EfCore/ProtectingTheApplicationLayer.md)
- [DatabaseFactory](../SystemX.Infrastructure.Persistence.EfCore/DatabaseFactory.md)
- [RepositoryFactory](../SystemX.Infrastructure.Persistence.EfCore/RepositoryFactory.md)
- [ConnectionFactory](../SystemX.Infrastructure.Persistence.EfCore/ConnectionFactory.md)
