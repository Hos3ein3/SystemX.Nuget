# SystemX.Infrastructure.MultiTenancy

## Purpose
Tenant resolution strategies (header, claim, host, route, query), tenant store implementations (in-memory, configuration, cached), `ITenantContextAccessor` implementation, ASP.NET Core multi-tenancy middleware.

Part of the SystemX library suite.

## Installation

```bash
dotnet add package SystemX.Infrastructure.MultiTenancy
```

## Usage

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SystemX.Infrastructure.MultiTenancy;
using SystemX.Infrastructure.MultiTenancy.Stores;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSystemXMultiTenancy()
    .AddTenantStore<InMemoryTenantStore>();

var app = builder.Build();
app.UseSystemXMultiTenancy();
```

## Documentation

- [TenantResolution](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.Infrastructure.MultiTenancy/TenantResolution.md)
- [TenantStores](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.Infrastructure.MultiTenancy/TenantStores.md)
- [MultiTenancySetup](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.Infrastructure.MultiTenancy/MultiTenancySetup.md)
