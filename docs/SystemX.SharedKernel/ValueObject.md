# ValueObject

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Domain`

## What it is
`ValueObject` is an abstract base class for immutable Domain-Driven Design value objects whose equality is based on their component values rather than identity.

## When to use it
- Use it for domain concepts with no independent lifecycle or identity (e.g., money, dates, coordinates, addresses).
- Use it to guarantee immutability and domain self-validation.
- Do NOT use it for entities that have an evolving lifecycle tracked by an identifier; use `Entity<TId>` instead.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Inherit from `ValueObject` and implement `GetEqualityComponents()` returning each property that participates in equality.

## Sample code
```csharp
using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Guards;

namespace Example;

public sealed class Money : ValueObject
{
    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = Guard.NotNullOrWhiteSpace(currency);
    }

    public decimal Amount { get; }
    public string Currency { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency.ToUpperInvariant();
    }
}

public sealed class Address : ValueObject
{
    public Address(string street, string city, string postalCode, string country)
    {
        Street = Guard.NotNullOrWhiteSpace(street);
        City = Guard.NotNullOrWhiteSpace(city);
        PostalCode = Guard.NotNullOrWhiteSpace(postalCode);
        Country = Guard.NotNullOrWhiteSpace(country);
    }

    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }
}
```

## Configuration (if any)
No external configuration required.

## Related
- [Entity.md](Entity.md)
