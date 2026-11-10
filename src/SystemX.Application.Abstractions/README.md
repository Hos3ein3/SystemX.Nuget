# SystemX.Application.Abstractions

## Purpose
The interfaces the application layer codes against: `ICommand`, `IQuery`, handler interfaces, dispatcher interfaces, `IUnitOfWork`, `ITransactionScope`, `IRepository<TEntity, TId>`, `IReadRepository<TEntity, TId>`, `IRepositoryFactory`, `IDatabaseFactory`, `IDatabaseSession`, `IDatabaseConnectionFactory`, `IReadConnectionFactory`, `IDomainEventDispatcher`, `ICurrentUser`, `ITenantContext`, `ITenantContextAccessor`, `ITenantStore`, `ITenantResolutionStrategy`, `ITenantConnectionResolver`, `ITenantSchemaProvider`, `ITenantProvisioner`, `ICurrentCulture`, `ICacheService`, `IEventBus`.

Part of the SystemX library suite.

## Installation

```bash
dotnet add package SystemX.Application.Abstractions
```

## Documentation

- [DatabaseFactories](../../docs/SystemX.Application.Abstractions/DatabaseFactories.md)
- [TenantAbstractions](../../docs/SystemX.Application.Abstractions/TenantAbstractions.md)
