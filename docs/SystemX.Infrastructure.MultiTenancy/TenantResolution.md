# Tenant Resolution

**Package:** `SystemX.Infrastructure.MultiTenancy`  
**Namespace:** `SystemX.Infrastructure.MultiTenancy`

## What it is
Transport-neutral tenant resolution pipeline running ordered strategies (`Header`, `Claim`, `Host`, `Route`, `Query`) to determine tenant identity from incoming HTTP requests or asynchronous messages.

## When to use it
- Use it to automatically map incoming requests to tenant configurations.
- Use `TenantResolutionContext.FromHeaders(headers)` when consuming messages from event buses or background jobs.
- Do NOT hardcode tenant lookups in application endpoints.

## Installation
`dotnet add package SystemX.Infrastructure.MultiTenancy`

## How to use it
Configure options or register custom resolution strategies during service registration.

## Sample code

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Infrastructure.MultiTenancy;
using SystemX.Infrastructure.MultiTenancy.Stores;
using SystemX.Infrastructure.MultiTenancy.Strategies;
using SystemX.SharedKernel.MultiTenancy;

namespace Example.TenantResolutionDoc;

public static class ResolutionSample
{
    public static async Task RunAsync()
    {
        ServiceCollection services = new();
        services.AddSystemXMultiTenancy(opt =>
        {
            opt.HeaderName = "X-Tenant-Key";
            opt.HostSuffix = "systemx.io";
        });

        // Seed in-memory store
        TenantInfo tenant = new(TenantId.Create("t-100"), "Acme", TenancyModel.Row, Identifier: "acme");
        services.AddSingleton<ITenantStore>(new InMemoryTenantStore([tenant]));

        ServiceProvider provider = services.BuildServiceProvider();
        TenantResolver resolver = provider.GetRequiredService<TenantResolver>();

        // Simulate incoming HTTP request context
        TenantResolutionContext context = new(
            headers: new Dictionary<string, string> { ["X-Tenant-Key"] = "acme" });

        TenantInfo? resolved = await resolver.ResolveAsync(context);
        Console.WriteLine($"Resolved Tenant: {resolved?.Name} ({resolved?.Id.Value})");
    }
}
```

## Related
- [TenantStores](TenantStores.md)
- [MultiTenancySetup](MultiTenancySetup.md)
