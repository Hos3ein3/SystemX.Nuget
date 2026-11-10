# TenancyModels

**Package:** `SystemX.Infrastructure.Persistence.EfCore`
**Namespace:** `SystemX.Infrastructure.Persistence.EfCore`

## What it is
Comprehensive multi-tenancy support across three isolation strategies: **Row-level isolation** (shared schema & database), **Schema-level isolation** (separate schema per tenant), and **Database-level isolation** (separate physical database per tenant).

## When to use it
- Use **Row** tenancy for high-density multi-tenant SaaS applications sharing a common database schema.
- Use **Schema** tenancy when logical database isolation is required within a single database instance (e.g. PostgreSQL schemas or SQL Server schemas).
- Use **Database** tenancy for enterprise customers demanding dedicated database files, separate disaster recovery, or distinct physical storage.

## Installation
`dotnet add package SystemX.Infrastructure.Persistence.EfCore`

## Architectural Details
- **Fixed Tenant per DbContext Scope:** Multi-tenancy resolution occurs exactly once when `SystemXDbContext` is constructed from DI. The tenant snapshot (`EffectiveModel`, `TenantSchema`, `CurrentTenantId`) remains immutable for the lifetime of that DbContext instance.
- **Why Contexts are Not Pooled:** Because the EF Core metadata model and default database schema dynamically adapt per tenant (specifically under `TenancyModel.Schema`), DbContext pooling (`AddDbContextPool`) is intentionally disabled.
- **Model Cache Considerations:** For the Schema model, `SystemXModelCacheKeyFactory` keys the EF Core model cache by `(DbContextType, TenancyModel, Schema)`. Each unique tenant schema generates a cached model.
- **HostSchema Requirement:** When using `TenancyModel.Schema`, shared platform entities (`ISharedEntity`) reside in a central host schema. `SystemXEfCoreOptions.HostSchema` must be configured.

## Sample code

### Multi-Tenancy Setup in Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Infrastructure.Persistence.EfCore.Context;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.MultiTenancy;

namespace InfrastructureSample;

public sealed class AppDbContext : SystemXDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options, SystemXDbContextDependencies<AppDbContext> dependencies)
        : base(options, dependencies) { }
}

public static class ServiceRegistration
{
    public static void ConfigureTenancyPersistence(IServiceCollection services)
    {
        services.AddSystemXDbContext<AppDbContext>(options =>
        {
            // Host database used for Row and Schema models
            options.HostConnectionString = "Server=localhost;Database=PlatformDb;Integrated Security=true;";
            options.ConfigureProvider = (builder, cs) => builder.UseSqlServer(cs);

            // Required for Schema tenancy model: shared entities are placed in this schema
            options.HostSchema = "shared";

            // If a request arrives without a resolved tenant, block tenant data access
            options.RequireTenant = true;
        });
    }
}
```

### Database Model Dynamic Routing

When `TenantInfo.Model` is `TenancyModel.Database`, `DatabaseConnectionFactory` and the DI registration automatically route connections to `TenantInfo.ConnectionString`:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.MultiTenancy;

namespace InfrastructureSample;

public sealed class EnterpriseProvisioningService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public EnterpriseProvisioningService(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task VerifyDedicatedDatabaseConnectionAsync(TenantInfo dedicatedTenant, CancellationToken cancellationToken)
    {
        // For Database model, resolves the tenant's private connection string
        await using var connection = await _connectionFactory.OpenConnectionAsync(dedicatedTenant, "Default", cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";
        await command.ExecuteScalarAsync(cancellationToken);
    }
}
```

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `HostConnectionString` | `null` | Connection string for Row and Schema models. |
| `HostSchema` | `null` | Schema name for `ISharedEntity` tables when using Schema tenancy. |
| `RequireTenant` | `true` | When true, unauthenticated/unresolved requests cannot access tenant rows. |
| `IgnoreTenancy` | `false` | When true, completely disables tenant isolation for host admin contexts. |

## Related
- [SystemXDbContext](SystemXDbContext.md)
- [QueryFilters](QueryFilters.md)
- [DatabaseFactory](DatabaseFactory.md)
- [ConnectionFactory](ConnectionFactory.md)
