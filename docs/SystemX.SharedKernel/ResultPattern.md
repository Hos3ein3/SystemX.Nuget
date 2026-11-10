# ResultPattern

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Results`

## What it is
`Result`, `Result<T>`, `Error`, and `ErrorType` implement the Result pattern for fluent functional error handling without relying on expensive exception throwing for predictable failures.

## When to use it
- Use it as the return type for use cases, command handlers, and business operations that can succeed or fail.
- Use it to categorize failures (Validation, NotFound, Conflict, Unauthorized, Forbidden).
- Do NOT use it for unhandled technical infrastructure failures; let genuine exceptions bubble up to error middleware.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Return `Result.Success(...)` or `Result.Failure(...)`. Chain results cleanly using `Map`, `Bind`, `Ensure`, or pattern match using `Match`.

## Sample code
```csharp
using System;
using System.Threading.Tasks;
using SystemX.SharedKernel.Results;

namespace Example;

public sealed class CreateUserHandler
{
    public Result<Guid> Handle(string email, string name)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Error.Validation("User.InvalidEmail", "Email address is required.");
        }

        var userId = Guid.NewGuid();
        return Result.Success(userId);
    }
}

public static class ResultPipelineExample
{
    public static string ProcessOrder(Result<decimal> amountResult)
    {
        return amountResult
            .Ensure(amount => amount > 0, Error.Validation("Order.NegativeAmount", "Amount must be positive."))
            .Map(amount => $"Total: {amount:C}")
            .Match(
                onSuccess: formatted => $"Success: {formatted}",
                onFailure: error => $"Failed [{error.Code}]: {error.Description}"
            );
    }
}
```

## Configuration (if any)
No external configuration required.

## Related
- [BusinessRules.md](BusinessRules.md)
- [Guard.md](Guard.md)
