# SystemX.Infrastructure.Validation

## Purpose
Runs FluentValidation on commands and queries through a handler decorator. Turns failures into `Result` with `ValidationError` (or throws `RequestValidationException`). Provides reusable rules for IDs.

Part of the SystemX library suite.

## Installation

```bash
dotnet add package SystemX.Infrastructure.Validation
```

## Usage

```csharp
using Microsoft.Extensions.DependencyInjection;
using SystemX.Infrastructure.Validation;

// Register CQRS handlers first, then add SystemX validation pipeline:
services.AddSystemXValidation([typeof(MyValidator).Assembly]);
```

## Documentation

- [Validation](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.Infrastructure.Validation/Validation.md)
- [ValidationRules](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.Infrastructure.Validation/ValidationRules.md)
