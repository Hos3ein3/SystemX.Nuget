# Multi-Tenancy Models

**Package:** `SystemX.SharedKernel`  
**Namespace:** `SystemX.SharedKernel.MultiTenancy`

## What it is
A comprehensive multi-tenancy model abstraction supporting Row-level, Schema-level, and Database-level data isolation. The isolation strategy is modeled as data on the tenant (`TenantInfo`), keeping domain entities decoupled from the underlying physical storage architecture.

## When to use it
- Use `TenancyModel.Row` when cost efficiency and unified schema migrations are top priorities for high tenant counts.
- Use `TenancyModel.Schema` when tenants require logical separation within the same database engine while maintaining shared server resources.
- Use `TenancyModel.Database` when tenants demand strict physical isolation, custom backup/restore schedules, compliance guarantees, or dedicated compute resources.
- Do NOT hardcode tenancy models inside domain entities; configure the model on `TenantInfo`.

## Isolation Comparison

| Dimension | Row Model | Schema Model | Database Model |
|---|---|---|---|
| **Data Isolation** | Soft (partitioned by column) | Logical (isolated schema) | Strict Physical (separate database) |
| **Infrastructure Cost** | Lowest (shared pool) | Moderate | Highest (dedicated instances/catalogs) |
| **Migrations** | Single migration script | Migrated per schema | Migrated per database |
| **Noisy Neighbor** | High risk | Moderate risk | Eliminated / Contained |
| **Point-in-Time Restore** | Complex (row extraction) | Moderate | Native database restore |

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Instantiate `TenantInfo` with the chosen `TenancyModel` and validate its properties:

## Sample code

```csharp
using System;
using SystemX.SharedKernel.MultiTenancy;
using SystemX.SharedKernel.Results;

namespace Example.MultiTenancyModelsDoc;

public static class TenancyModelSample
{
    public static void Run()
    {
        // 1. Row-level tenant
        TenantInfo rowTenant = new(
            Id: TenantId.Create("tenant-alpha"),
            Name: "Alpha Corp",
            Model: TenancyModel.Row,
            Identifier: "alpha");

        // 2. Schema-level tenant
        TenantInfo schemaTenant = new(
            Id: TenantId.Create("tenant-beta"),
            Name: "Beta Corp",
            Model: TenancyModel.Schema,
            Identifier: "beta",
            Schema: "tenant_beta");

        // 3. Database-level tenant
        TenantInfo dbTenant = new(
            Id: TenantId.Create("tenant-gamma"),
            Name: "Gamma Corp",
            Model: TenancyModel.Database,
            Identifier: "gamma",
            ConnectionString: "Server=db-gamma;Database=TenantGamma;User Id=app;Password=secret;");

        Result result = dbTenant.Validate();
        Console.WriteLine($"Is valid: {result.IsSuccess}");

        // ConnectionString is safely redacted in ToString()
        Console.WriteLine(dbTenant.ToString());
    }
}
```

## Related
- [TenantMarkers](TenantMarkers.md)
- [TenantAbstractions](../SystemX.Application.Abstractions/TenantAbstractions.md)
