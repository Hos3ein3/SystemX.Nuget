# SystemX.SharedKernel

## Purpose
Domain and application building blocks shared by all apps: Entity, AggregateRoot, ValueObject, Enumeration, domain events, business rules, Result and Error, Guard, IClock, strongly-typed IDs, specifications, pagination types, multi-tenancy model/info types, concurrency markers, entity markers, translations, audit/soft-delete markers.

Part of the SystemX library suite.

## Installation

```bash
dotnet add package SystemX.SharedKernel
```

## Usage

```csharp
using System;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.Results;
using SystemX.SharedKernel.Guards;

public sealed record OrderId(Guid Value) : GuidId(Value), IGuidIdFactory<OrderId>
{
    public static OrderId Create(Guid value) => new(value);
    public static OrderId New() => GuidIds.New<OrderId>();
}
```

## Documentation

Comprehensive guides and compilable examples for each building block:

- [Entity](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Entity.md)
- [AggregateRoot](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/AggregateRoot.md)
- [DomainEvents](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/DomainEvents.md)
- [ValueObject](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/ValueObject.md)
- [Enumeration](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Enumeration.md)
- [BusinessRules](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/BusinessRules.md)
- [Auditing](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Auditing.md)
- [SoftDelete](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/SoftDelete.md)
- [MultiTenancy](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/MultiTenancy.md)
- [MultiTenancyModels](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/MultiTenancyModels.md)
- [TenantMarkers](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/TenantMarkers.md)
- [Concurrency](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Concurrency.md)
- [EntityMarkers](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/EntityMarkers.md)
- [Translations](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Translations.md)
- [ResultPattern](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/ResultPattern.md)
- [ValidationResults](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/ValidationResults.md)
- [Guard](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Guard.md)
- [Clock](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Clock.md)
- [StronglyTypedIds](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/StronglyTypedIds.md)
- [Specifications](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Specifications.md)
- [Pagination](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Pagination.md)
