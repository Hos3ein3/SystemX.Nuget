# SystemX.Infrastructure.Serialization

## Purpose
`System.Text.Json` support: converters and type converters for strongly-typed IDs and other SystemX types.

Part of the SystemX library suite.

## Installation

```bash
dotnet add package SystemX.Infrastructure.Serialization
```

## Usage

```csharp
using System.Text.Json;
using SystemX.Infrastructure.Serialization;

var options = new JsonSerializerOptions().AddSystemXConverters();
```

## Documentation

- [StronglyTypedIdJson](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.Infrastructure.Serialization/StronglyTypedIdJson.md)
