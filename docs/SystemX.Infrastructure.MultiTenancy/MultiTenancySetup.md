# Multi-Tenancy Setup and Program.cs Integration

**Package:** `SystemX.Infrastructure.MultiTenancy`  
**Namespace:** `SystemX.Infrastructure.MultiTenancy`

## What it is
Complete DI setup and middleware pipeline integration demonstrating Row, Schema, and Database tenancy models in ASP.NET Core applications.

## When to use it
- Use `builder.Services.AddSystemXMultiTenancy()` in your host `Program.cs`.
- Use `app.UseSystemXMultiTenancy()` early in the middleware pipeline (before routing and endpoint execution).
- Do NOT place the middleware after authentication if claims-based resolution is desired; place after authentication but before endpoints.

## Installation
`dotnet add package SystemX.Infrastructure.MultiTenancy`

## How to use it
Configure options and middleware in `Program.cs`.

## Sample code

```csharp
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Infrastructure.MultiTenancy;
using SystemX.Infrastructure.MultiTenancy.Stores;
using SystemX.SharedKernel.MultiTenancy;

namespace Example.MultiTenancySetupDoc;

public static class HostSetupSample
{
    public static void ConfigureApp(WebApplicationBuilder builder)
    {
        // 1. Add multi-tenancy services
        builder.Services.AddSystemXMultiTenancy(options =>
        {
            options.RequireTenant = true;
            options.HeaderName = "X-Tenant-Id";
            options.DefaultModel = TenancyModel.Row;
        })
        .AddTenantStore<InMemoryTenantStore>();

        WebApplication app = builder.Build();

        // 2. Use middleware in request pipeline
        app.UseSystemXMultiTenancy();

        // 3. Define endpoint accessing current tenant
        app.MapGet("/api/tenant", (ITenantContext context) =>
        {
            TenantInfo tenant = context.RequireTenant();
            return Results.Ok(new { tenant.Id.Value, tenant.Name, Model = tenant.Model.ToString() });
        });
    }
}
```

## Configuration
| Option | Default | Description |
|---|---|---|
| `DefaultModel` | `TenancyModel.Row` | Default isolation model when unspecified. |
| `RequireTenant` | `true` | Whether requests must resolve a tenant (fails with 400 ProblemDetails if missing). |
| `HeaderName` | `"X-Tenant-Id"` | Header inspected by `HeaderTenantResolutionStrategy`. |
| `ClaimType` | `"tenant_id"` | Claim inspected by `ClaimTenantResolutionStrategy`. |
| `QueryKey` | `"tenant"` | Query string parameter name. |
| `RouteKey` | `"tenant"` | Route value parameter name. |
| `SchemaNameFormat` | `"tenant_{0}"` | Schema naming format for Schema model. |

## Related
- [TenantResolution](TenantResolution.md)
- [TenancyAndConcurrency](../SystemX.Infrastructure.Persistence.EfCore/TenancyAndConcurrency.md)
