# Guard

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Guards`

## What it is
`Guard` provides static assertion and defensive programming utilities that throw clear exceptions when arguments or prerequisites fail.

## When to use it
- Use it to validate method and constructor arguments at public boundaries.
- Use it to guard against `null`, empty strings, empty collections, default values, or out-of-range inputs.
- Do NOT use it for complex business rule validation that requires state inspection across aggregates; use `IBusinessRule` and `CheckRule` instead.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Call `Guard.NotNull`, `Guard.NotNullOrWhiteSpace`, `Guard.NotDefault`, `Guard.InRange`, etc. inside constructors or methods.

## Sample code
```csharp
using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Guards;

namespace Example;

public sealed class CustomerProfile
{
    public CustomerProfile(string fullName, int age, decimal creditLimit, IEnumerable<string> roles)
    {
        FullName = Guard.NotNullOrWhiteSpace(fullName);
        Age = Guard.InRange(age, 18, 120);
        CreditLimit = Guard.Positive(creditLimit);
        Roles = Guard.NotNullOrEmpty(roles);
    }

    public string FullName { get; }
    public int Age { get; }
    public decimal CreditLimit { get; }
    public IEnumerable<string> Roles { get; }
}
```

## Configuration (if any)
No external configuration required.

## Related
- [BusinessRules.md](BusinessRules.md)
- [ResultPattern.md](ResultPattern.md)
