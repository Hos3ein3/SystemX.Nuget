# Tenant Stores

**Package:** `SystemX.Infrastructure.MultiTenancy`  
**Namespace:** `SystemX.Infrastructure.MultiTenancy.Stores`

## What it is
Store implementations for retrieving tenant configurations: `InMemoryTenantStore` (testing & fast setups), `ConfigurationTenantStore` (appsettings.json integration), and `CachedTenantStore` (caching decorator).

## When to use it
- Use `InMemoryTenantStore` for unit tests and local mock environments.
- Use `ConfigurationTenantStore` for static tenant definitions in configuration files.
- Use `CachedTenantStore` to wrap database-backed or remote tenant stores to avoid per-request database lookups.
- Do NOT bypass tenant caching in high-throughput production services.

## Installation
`dotnet add package SystemX.Infrastructure.MultiTenancy`

## How to use it
Register the desired store using `.AddTenantStore<TStore>()`.

## Sample code

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Infrastructure.MultiTenancy;
using SystemX.Infrastructure.MultiTenancy.Stores;
using SystemX.SharedKernel.MultiTenancy;

namespace Example.TenantStoresDoc;

public static class StoreSample
{
    public static async Task RunAsync()
    {
        ServiceCollection services = new();
        services.AddSystemXMultiTenancy()
            .AddTenantStore<InMemoryTenantStore>();

        ServiceProvider provider = services.BuildServiceProvider();
        ITenantStore store = provider.GetRequiredService<ITenantStore>();

        if (store is InMemoryTenantStore memoryStore)
        {
            memoryStore.AddOrUpdate(new TenantInfo(
                TenantId.Create("cust-1"),
                "Customer One",
                TenancyModel.Row,
                Identifier: "cust1"));
        }

        TenantInfo? found = await store.FindByIdentifierAsync("cust1");
        Console.WriteLine($"Found: {found?.Name}");
    }
}
```

## Related
- [TenantResolution](TenantResolution.md)
- [MultiTenancySetup](MultiTenancySetup.md)
