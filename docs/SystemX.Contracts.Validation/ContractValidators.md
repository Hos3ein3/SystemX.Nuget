# Contract Validators

**Package:** `SystemX.Contracts.Validation`  
**Namespace:** `SystemX.Contracts.Validation`

## What it is
FluentValidation rules for shared request contracts and DTOs (such as `PagedRequest`), enabling consistent input validation on both client and server without polluting the core contracts package.

## When to use it
- Use it to validate request DTOs received in Web APIs or API gateways.
- Use `ContractsValidationAssemblyMarker` when scanning assemblies for validators during service registration.
- Do NOT add database calls or business rules here; these validators check structure and syntax only.

## Installation
`dotnet add package SystemX.Contracts.Validation`

## How to use it
Register contracts validation via DI or validate directly with `PagedRequestValidator`.

## Sample code

```csharp
using System;
using FluentValidation.Results;
using SystemX.Contracts.Pagination;
using SystemX.Contracts.Validation;

namespace Example.ContractsValidationDoc;

public static class ValidationSample
{
    public static void Run()
    {
        PagedRequestValidator validator = new();
        PagedRequest invalidRequest = new(Page: 0, PageSize: 500, SortBy: "Drop Table--");

        ValidationResult result = validator.Validate(invalidRequest);
        Console.WriteLine($"Is valid: {result.IsValid}");
        foreach (ValidationFailure error in result.Errors)
        {
            Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage} ({error.ErrorCode})");
        }
    }
}
```

## Related
- [Pagination](../SystemX.Contracts/Pagination.md)
