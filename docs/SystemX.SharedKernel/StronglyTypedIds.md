# Strongly-Typed IDs

**Package:** `SystemX.SharedKernel`  
**Namespace:** `SystemX.SharedKernel.Identifiers`

## What it is
A strongly-typed identifier framework preventing accidental mixing of entity IDs (e.g., passing `CustomerId` where `OrderId` is expected) with zero boilerplate and default support for time-ordered UUIDv7.

## When to use it
- Use it for all domain entity identifiers instead of raw `Guid`, `int`, `long`, or `string`.
- Use UUIDv7 as the default for GUID identifiers when time-ordered locality in database indices is desired (especially on PostgreSQL, MySQL, and distributed document stores).
- Use `IntId` or `LongId` when integrating with existing relational tables using integer auto-increment keys.
- Do NOT use strongly-typed IDs directly on public network contracts (DTOs) if loose coupling with external clients is desired; instead validate primitives in contracts and parse them into strongly-typed IDs in the application layer.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
1. Inherit your identifier record from `GuidId`, `IntId`, `LongId`, or `StringId`.
2. Implement the corresponding factory interface (`IGuidIdFactory<TSelf>`, `IIntIdFactory<TSelf>`, etc.) to enable reflection-free creation.
3. Use the static helpers `GuidIds`, `IntIds`, `LongIds`, or `StringIds` for instantiation and parsing.

> [!NOTE]
> Database value converters for EF Core and MongoDB serialization are planned for `SystemX.Infrastructure.Persistence.EfCore` and `SystemX.Infrastructure.Persistence.Mongo` in subsequent releases.

## Sample code

```csharp
using System;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.Results;

namespace Example.StronglyTypedIdsDoc;

public sealed record OrderId(Guid Value) : GuidId(Value), IGuidIdFactory<OrderId>
{
    public static OrderId Create(Guid value) => new(value);
    public static OrderId New() => GuidIds.New<OrderId>();
    public static OrderId NewV4() => GuidIds.New<OrderId>(GuidVersion.V4);
}

public sealed record CustomerId(Guid Value) : GuidId(Value), IGuidIdFactory<CustomerId>
{
    public static CustomerId Create(Guid value) => new(value);
}

public sealed record LegacyInvoiceId(int Value) : IntId(Value), IIntIdFactory<LegacyInvoiceId>
{
    public static LegacyInvoiceId Create(int value) => new(value);
}

public static class UsageSample
{
    public static void Run()
    {
        OrderId orderId = OrderId.New();
        string compact = orderId.ToString(UuidFormat.Compact);
        string base64Url = orderId.ToString(UuidFormat.Base64Url);

        if (GuidIds.TryParse(base64Url, out OrderId? parsed))
        {
            Console.WriteLine(parsed.Value);
        }

        Result<OrderId> result = GuidIds.ParseResult<OrderId>("invalid-uuid", "Order.InvalidId");
        Console.WriteLine(result.IsFailure);
    }
}
```

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `GuidVersion` | `V7` | Version of UUID to generate (`V7` time-ordered or `V4` random). |
| `UuidFormat` | `Default` | String output format (`Default`, `Compact`, `Braces`, `Base64Url`). |

## Related
- [Specifications](Specifications.md)
- [ResultPattern](ResultPattern.md)
