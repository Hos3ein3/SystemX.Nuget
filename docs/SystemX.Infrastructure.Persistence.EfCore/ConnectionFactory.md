# ConnectionFactory

**Package:** `SystemX.Infrastructure.Persistence.EfCore`
**Namespace:** `SystemX.Infrastructure.Persistence.EfCore.Connections`

## What it is
Implementation of `IDatabaseConnectionFactory` (`DatabaseConnectionFactory`) providing connection string resolution and raw `DbConnection` creation for high-performance direct SQL queries without exposing EF Core.

## When to use it
- Use it when executing high-throughput queries or complex SQL scripts (e.g., using Dapper) while respecting multi-tenancy database routing.
- Use named connections when querying auxiliary host databases (e.g., "Reporting", "Audit").
- Do NOT use it for domain aggregate mutations; aggregate state must be managed via `IRepository<TEntity, TId>` and `IUnitOfWork`.

## Installation
`dotnet add package SystemX.Infrastructure.Persistence.EfCore`

## How to use it
Register the provider and connection creator delegate in `AddSystemXDbContext`:

```csharp
services.AddSystemXDbContext<AppDbContext>(options =>
{
    options.HostConnectionString = "Server=localhost;Database=HostDb;Integrated Security=true;";
    options.ConfigureProvider = (builder, cs) => builder.UseSqlServer(cs);
    options.CreateConnection = cs => new Microsoft.Data.SqlClient.SqlConnection(cs);
    options.NamedConnectionStrings["Reporting"] = "Server=localhost;Database=ReportingDb;Integrated Security=true;";
});
```

Inject `IDatabaseConnectionFactory` into your query handler or reporting service.

## Security Rule
Connection strings are secrets that often contain credentials or sensitive network topology. SystemX guarantees that connection strings are never included in exception messages or logged by internal factories. Applications must never log connection strings.

## Sample code

### High-Performance Dapper Query Sample

```csharp
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.MultiTenancy;

namespace ApplicationSample;

public sealed record CustomerSummaryDto(string Id, string Name, string Email);

public sealed class CustomerQueryService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public CustomerQueryService(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<CustomerSummaryDto>> GetCustomerSummariesAsync(
        TenantInfo? tenant,
        CancellationToken cancellationToken)
    {
        // Opens the correct host or database-model tenant connection. The caller owns and disposes it.
        await using var connection = await _connectionFactory.OpenConnectionAsync(tenant, "Default", cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Email FROM Customers WHERE IsDeleted = 0";

        var list = new List<CustomerSummaryDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(new CustomerSummaryDto(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2)));
        }

        return list.AsReadOnly();
    }
}
```

### Named Connection Sample

```csharp
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;

namespace ApplicationSample;

public sealed class AuditLogQueryService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public AuditLogQueryService(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> GetRecentAuditEventsCountAsync(CancellationToken cancellationToken)
    {
        // Resolves the named connection string "Audit" configured on SystemXEfCoreOptions
        await using var connection = await _connectionFactory.OpenConnectionAsync(null, "Audit", cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM AuditEvents WHERE TimestampUtc >= CURRENT_TIMESTAMP - 1";

        var count = await command.ExecuteScalarAsync(cancellationToken);
        return System.Convert.ToInt32(count);
    }
}
```

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `CreateConnection` | `null` | Delegate `Func<string, DbConnection>` creating closed database connections. Set by provider packages. |
| `NamedConnectionStrings` | Empty | Dictionary of auxiliary named connection strings (case-insensitive keys). |
| `HostConnectionString` | `null` | Connection string of the host database. |

## Related
- [DatabaseFactories](../SystemX.Application.Abstractions/DatabaseFactories.md)
- [DatabaseFactory](DatabaseFactory.md)
- [UnitOfWork](UnitOfWork.md)
