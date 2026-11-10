# Validation Results

**Package:** `SystemX.SharedKernel`  
**Namespace:** `SystemX.SharedKernel.Results`

## What it is
Validation failure models integrated directly into the `Result` and `Error` pattern, providing structured, property-level error reporting without throwing exceptions.

## When to use it
- Use `ValidationError` and `ValidationFailure` when user input or command payload fails validation rules.
- Use `Error.Validation(failures)` to return multiple property-specific errors to clients.
- Do NOT throw exceptions for predictable validation failures; return `Result.Failure(validationError)`.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
1. Collect failure details into `ValidationFailure` records.
2. Return a failed `Result` or `Result<T>` containing `Error.Validation(failures)`.
3. Check `if (result.Error is ValidationError validationError)` in callers to extract detailed field errors.

## Sample code

```csharp
using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Results;

namespace Example.ValidationResultsDoc;

public static class ValidationResultSample
{
    public static Result<int> RegisterUser(string? username, int age)
    {
        List<ValidationFailure> failures = [];

        if (string.IsNullOrWhiteSpace(username))
        {
            failures.Add(new ValidationFailure("Username", "User.EmptyUsername", "Username is required."));
        }

        if (age < 18)
        {
            failures.Add(new ValidationFailure("Age", "User.Underage", "User must be at least 18 years old."));
        }

        if (failures.Count > 0)
        {
            return Result.Failure<int>(Error.Validation(failures));
        }

        return Result<int>.Success(100);
    }

    public static void Run()
    {
        Result<int> outcome = RegisterUser("", 15);
        if (outcome.Error is ValidationError valError)
        {
            foreach (ValidationFailure failure in valError.Failures)
            {
                Console.WriteLine($"{failure.PropertyName}: {failure.Message} ({failure.Code})");
            }
        }
    }
}
```

## Related
- [ResultPattern](ResultPattern.md)
