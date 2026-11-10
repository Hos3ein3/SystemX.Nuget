# Strongly-Typed ID JSON Serialization

**Package:** `SystemX.Infrastructure.Serialization`  
**Namespace:** `SystemX.Infrastructure.Serialization`

## What it is
System.Text.Json converters and type converters for strongly-typed identifiers (`StronglyTypedId<T>`), serializing identifiers transparently as raw primitives (strings, numbers) and supporting dictionary keys as well as route/query model binding.

## When to use it
- Use it for internal communication, message broker events, cache entries, or internal domain persistence using JSON.
- Recommended flow for public network APIs: define public DTOs using primitive types (`string`, `Guid`, `int`), validate them using FluentValidation, and construct strongly-typed IDs in the application layer. Use these JSON converters for direct serialization when appropriate.
- Do NOT serialize complex entity graphs containing circular references.

## Installation
`dotnet add package SystemX.Infrastructure.Serialization`

## How to use it
1. Call `options.AddSystemXConverters()` on `JsonSerializerOptions`, or call `services.AddSystemXSerialization()` in your DI setup.
2. For ASP.NET Core route and query string binding, register `StronglyTypedIdTypeDescriptor.Register<TId>()`.

## Sample code

```csharp
using System;
using System.Collections.Generic;
using System.Text.Json;
using SystemX.Infrastructure.Serialization;
using SystemX.SharedKernel.Identifiers;

namespace Example.StronglyTypedIdJsonDoc;

public sealed record CustomerId(Guid Value) : GuidId(Value), IGuidIdFactory<CustomerId>
{
    public static CustomerId Create(Guid value) => new(value);
}

public sealed record AccountState(CustomerId Id, string Name, Dictionary<CustomerId, string> Roles);

public static class JsonSample
{
    public static void Run()
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
            .AddSystemXConverters(new StronglyTypedIdJsonOptions { WriteFormat = UuidFormat.Compact });

        CustomerId id = CustomerId.Create(Guid.NewGuid());
        AccountState account = new(id, "Alice", new Dictionary<CustomerId, string> { [id] = "Admin" });

        string json = JsonSerializer.Serialize(account, options);
        Console.WriteLine($"Serialized: {json}");

        AccountState? deserialized = JsonSerializer.Deserialize<AccountState>(json, options);
        Console.WriteLine($"Deserialized ID: {deserialized?.Id.Value}");

        // Model binding registration
        StronglyTypedIdTypeDescriptor.Register<CustomerId>();
    }
}
```

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `WriteFormat` | `Default` | String format used when writing UUIDs (`Default`, `Compact`, `Braces`, `Base64Url`). |

## Related
- [StronglyTypedIds](../SystemX.SharedKernel/StronglyTypedIds.md)
