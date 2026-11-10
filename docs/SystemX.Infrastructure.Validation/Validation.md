# Validation Pipeline and Decorators

**Package:** `SystemX.Infrastructure.Validation`  
**Namespace:** `SystemX.Infrastructure.Validation`

## What it is
Validation execution pipeline that automatically decorates CQRS command and query handlers, converting FluentValidation failures into structured `Result` objects containing `ValidationError` (or throwing `RequestValidationException` for handlers not returning `Result`).

## When to use it
- Use it to enforce automatic validation of all incoming commands and queries before they reach business handlers.
- Use it to guarantee that invalid requests never execute handler business logic.
- Do NOT call validators manually inside handlers; let the decorator intercept the pipeline.

## Installation
`dotnet add package SystemX.Infrastructure.Validation`

## How to use it
1. Register your CQRS command and query handlers first in the DI container.
2. Call `services.AddSystemXValidation(assemblies)` to scan for validators and decorate handlers.
3. If validation fails for a handler returning `Result` or `Result<T>`, the decorator immediately returns `Result.Failure(validationError)`.

## Sample code

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SystemX.Application.Abstractions.Cqrs;
using SystemX.Infrastructure.Validation;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.Results;

namespace Example.ValidationDoc;

public sealed record OrderId(Guid Value) : GuidId(Value), IGuidIdFactory<OrderId>
{
    public static OrderId Create(Guid value) => new(value);
}

public sealed record CreateOrderCommand(string CustomerName, decimal Amount) : ICommand<Result<OrderId>>;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithErrorCode("Order.CustomerNameRequired");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithErrorCode("Order.AmountMustBePositive");
    }
}

public sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Result<OrderId>>
{
    public Task<Result<OrderId>> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        OrderId id = OrderId.Create(Guid.NewGuid());
        return Task.FromResult(Result<OrderId>.Success(id));
    }
}

public static class PipelineSample
{
    public static async Task RunAsync()
    {
        ServiceCollection services = new();

        // 1. Register handler
        services.AddScoped<ICommandHandler<CreateOrderCommand, Result<OrderId>>, CreateOrderCommandHandler>();

        // 2. Register validation pipeline
        services.AddSystemXValidation([typeof(CreateOrderCommandValidator).Assembly]);

        ServiceProvider provider = services.BuildServiceProvider();
        var handler = provider.GetRequiredService<ICommandHandler<CreateOrderCommand, Result<OrderId>>>();

        // Invalid command triggers validation failure result automatically
        CreateOrderCommand invalidCommand = new("", -5m);
        Result<OrderId> result = await handler.HandleAsync(invalidCommand);

        if (result.Error is ValidationError validationError)
        {
            Console.WriteLine($"Validation failed with {validationError.Failures.Count} errors.");
        }
    }
}
```

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `StopOnFirstFailure` | `false` | Whether validation halts immediately on the first failure. |
| `CascadeMode` | `Continue` | FluentValidation cascade mode across rules. |
| `IncludeErrorCodes` | `true` | Whether FluentValidation error codes are included in failures. |

## Related
- [ValidationRules](ValidationRules.md)
- [ValidationResults](../SystemX.SharedKernel/ValidationResults.md)
