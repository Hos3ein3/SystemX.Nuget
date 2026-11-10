# SystemX.Infrastructure.Persistence.EfCore

## Purpose
Provider-neutral EF Core base: multi-tenancy models (Row, Schema, Database), concurrency handling, entity markers, `UnitOfWork`, generic repositories, specification evaluator, interceptors (audit, soft delete, tenant, domain-event dispatch), strongly-typed ID value converters.

Part of the SystemX library suite.

## Installation

```bash
dotnet add package SystemX.Infrastructure.Persistence.EfCore
```

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Infrastructure.Persistence.EfCore.Interceptors;
using SystemX.Infrastructure.Persistence.EfCore.Model;

public class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplySystemXConventions(tenantContext);
    }
}
```

## Documentation

- [TenancyAndConcurrency](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.Infrastructure.Persistence.EfCore/TenancyAndConcurrency.md)
