# Tenant Abstractions

**Package:** `SystemX.Application.Abstractions`  
**Namespace:** `SystemX.Application.Abstractions.MultiTenancy`

## What it is
Application-layer contracts for accessing ambient tenant context (`ITenantContext`, `ITenantContextAccessor`), loading tenant metadata (`ITenantStore`), resolving identifiers (`ITenantResolutionStrategy`), and abstracting database provisioning.

## When to use it
- Use `ITenantContext` in application handlers and domain services to inspect the currently active tenant.
- Use `ITenantContextAccessor.BeginScope(tenant)` in background workers, message consumers, and job runners to execute code under an explicit tenant.
- Do NOT directly reference infrastructure implementations like `TenantResolver` or `MultiTenancyMiddleware`.

## Installation
`dotnet add package SystemX.Application.Abstractions`

## How to use it
Inject `ITenantContext` into your handlers to obtain the current tenant.

## Sample code

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Cqrs;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;
using SystemX.SharedKernel.Results;

namespace Example.TenantAbstractionsDoc;

public sealed record ProcessTenantDataCommand : ICommand<Result>;

public sealed class ProcessTenantDataCommandHandler(ITenantContext tenantContext)
    : ICommandHandler<ProcessTenantDataCommand, Result>
{
    public Task<Result> HandleAsync(ProcessTenantDataCommand command, CancellationToken cancellationToken = default)
    {
        // Enforce tenant presence
        TenantInfo tenant = tenantContext.RequireTenant();
        Console.WriteLine($"Processing command for tenant '{tenant.Name}' under {tenant.Model} model.");

        return Task.FromResult(Result.Success());
    }
}

public static class BackgroundWorkerSample
{
    public static void ExecuteInTenantScope(ITenantContextAccessor accessor, TenantInfo tenant)
    {
        using (accessor.BeginScope(tenant))
        {
            Console.WriteLine($"Ambient tenant is now: {accessor.Tenant?.Name}");
        }
    }
}
```

## Related
- [MultiTenancyModels](../SystemX.SharedKernel/MultiTenancyModels.md)
- [TenantResolution](../SystemX.Infrastructure.MultiTenancy/TenantResolution.md)
