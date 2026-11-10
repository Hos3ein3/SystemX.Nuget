# Validation Rule Extensions

**Package:** `SystemX.Infrastructure.Validation`  
**Namespace:** `SystemX.Infrastructure.Validation`

## What it is
Reusable FluentValidation rule builder extensions specifically tailored for strongly-typed identifiers, UUID formats (including UUIDv7), and common primitive constraints.

## When to use it
- Use `MustBeValidGuid()` when validating incoming strings in D, N, B, or Base64Url formats.
- Use `MustBeGuidV7()` to enforce time-ordered UUIDv7 identifiers.
- Use `MustBePositiveId()` for validating `int` or `long` database surrogate identifiers.
- Use `NotEmptyGuid()` and `ValidStringId(maxLength)` for standard identifier assertions.

## Installation
`dotnet add package SystemX.Infrastructure.Validation`

## How to use it
In your `AbstractValidator<T>`, invoke the extension methods directly on `RuleFor(...)`.

## Sample code

```csharp
using System;
using FluentValidation;
using SystemX.Infrastructure.Validation;

namespace Example.ValidationRulesDoc;

public sealed record UpdateProductCommand(
    string ProductIdString,
    Guid CatalogId,
    int SupplierId,
    string Sku);

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductIdString)
            .MustBeValidGuid()
            .MustBeGuidV7();

        RuleFor(x => x.CatalogId)
            .NotEmptyGuid();

        RuleFor(x => x.SupplierId)
            .MustBePositiveId();

        RuleFor(x => x.Sku)
            .ValidStringId(maxLength: 20);
    }
}
```

## Related
- [Validation](Validation.md)
- [StronglyTypedIds](../SystemX.SharedKernel/StronglyTypedIds.md)
