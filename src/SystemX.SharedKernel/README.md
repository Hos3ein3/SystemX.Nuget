# SystemX.SharedKernel

Domain and application primitives (Entities, AggregateRoots, ValueObjects, Domain Events, Business Rules, Auditing, Soft Delete, Multi-Tenancy, Result Pattern, Guards, and Clock abstractions) for building clean, domain-driven .NET applications.

Part of the SystemX library suite.

## Installation

```bash
dotnet add package SystemX.SharedKernel
```

## Usage

```csharp
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Results;
using SystemX.SharedKernel.Guards;

// Inherit from Entity, AggregateRoot, or ValueObject
public sealed class OrderId : ValueObject
{
    public OrderId(Guid value) => Value = Guard.NotDefault(value);
    public Guid Value { get; }
    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }
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
- [ResultPattern](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/ResultPattern.md)
- [Guard](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Guard.md)
- [Clock](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.SharedKernel/Clock.md)
