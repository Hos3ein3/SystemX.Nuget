# SystemX.Infrastructure.Persistence.EfCore

## Purpose
Provider-neutral EF Core base: multi-tenancy models (Row, Schema, Database), concurrency handling, entity markers, base DbContext with unit of work, save pipeline (audit, soft delete, tenant, domain events), query filters, strongly-typed ID value converters, specification evaluator, generic repositories, factories (`IRepositoryFactory`, `IDatabaseFactory`, `IDatabaseSession`), connection factory (`IDatabaseConnectionFactory`), DI registration.

Part of the SystemX library suite.

## Installation

```bash
dotnet add package SystemX.Infrastructure.Persistence.EfCore
```

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SystemX.Infrastructure.Persistence.EfCore.Context;

public class AppDbContext : SystemXDbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        SystemXDbContextDependencies<AppDbContext> dependencies)
        : base(options, dependencies)
    {
    }
}

// In Program.cs:
builder.Services.AddSystemXDbContext<AppDbContext>(options =>
{
    options.HostConnectionString = builder.Configuration.GetConnectionString("Default");
    options.ConfigureProvider = (b, cs) => b.UseSqlServer(cs);
});
```

## Documentation

- [ProtectingTheApplicationLayer](../../docs/SystemX.Infrastructure.Persistence.EfCore/ProtectingTheApplicationLayer.md)
- [DatabaseFactory](../../docs/SystemX.Infrastructure.Persistence.EfCore/DatabaseFactory.md)
- [RepositoryFactory](../../docs/SystemX.Infrastructure.Persistence.EfCore/RepositoryFactory.md)
- [ConnectionFactory](../../docs/SystemX.Infrastructure.Persistence.EfCore/ConnectionFactory.md)
- [SystemXDbContext](../../docs/SystemX.Infrastructure.Persistence.EfCore/SystemXDbContext.md)
- [SavePipeline](../../docs/SystemX.Infrastructure.Persistence.EfCore/SavePipeline.md)
- [QueryFilters](../../docs/SystemX.Infrastructure.Persistence.EfCore/QueryFilters.md)
- [TenancyModels](../../docs/SystemX.Infrastructure.Persistence.EfCore/TenancyModels.md)
- [Concurrency](../../docs/SystemX.Infrastructure.Persistence.EfCore/Concurrency.md)
- [StronglyTypedIds](../../docs/SystemX.Infrastructure.Persistence.EfCore/StronglyTypedIds.md)
- [Repositories](../../docs/SystemX.Infrastructure.Persistence.EfCore/Repositories.md)
- [UnitOfWork](../../docs/SystemX.Infrastructure.Persistence.EfCore/UnitOfWork.md)
