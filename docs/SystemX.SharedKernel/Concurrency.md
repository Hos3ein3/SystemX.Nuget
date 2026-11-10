# Concurrency

**Package:** `SystemX.SharedKernel`  
**Namespace:** `SystemX.SharedKernel.Concurrency`

## What it is
Provider-neutral optimistic concurrency markers, base classes, and exception types for detecting and resolving simultaneous database writes.

## When to use it
- Use `IHasVersion` (or `VersionedAggregateRoot<TId>`) for cross-database numeric version tracking (works seamlessly on SQL Server, PostgreSQL, and MongoDB).
- Use `IHasConcurrencyStamp` (or `StampedAggregateRoot<TId>`) for distributed or web API scenarios where concurrency tokens double as HTTP `ETag` headers.
- Use `IHasRowVersion` when binding to native SQL Server `rowversion` / `timestamp` columns.
- Do NOT use multiple concurrency strategies on the same entity.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Inherit from `VersionedAggregateRoot<TId>` or `StampedAggregateRoot<TId>`, or implement `IHasVersion` / `IHasConcurrencyStamp`.

## Sample code

```csharp
using System;
using SystemX.SharedKernel.Concurrency;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.Results;

namespace Example.ConcurrencyDoc;

public sealed record AccountId(Guid Value) : GuidId(Value), IGuidIdFactory<AccountId>
{
    public static AccountId Create(Guid value) => new(value);
}

public sealed class BankAccount : VersionedAggregateRoot<AccountId>
{
    public decimal Balance { get; private set; }

    public BankAccount(AccountId id, decimal initialBalance) : base(id)
    {
        Balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        Balance += amount;
    }
}

public static class ConcurrencySample
{
    public static Result HandleUpdate(Action updateAction)
    {
        try
        {
            updateAction();
            return Result.Success();
        }
        catch (ConcurrencyException ex)
        {
            // Returns a structured 409 Conflict error
            return Result.Failure(ConcurrencyException.ConcurrencyConflict(ex.EntityType ?? "Entity"));
        }
    }

    public static void Run()
    {
        BankAccount account = new(AccountId.Create(Guid.NewGuid()), 100m);
        account.Deposit(50m);

        Result result = HandleUpdate(() =>
        {
            throw new ConcurrencyException("BankAccount", account.Id.ToString());
        });

        Console.WriteLine($"Result code: {result.Error.Code}, type: {result.Error.Type}");
    }
}
```

## Related
- [EntityMarkers](EntityMarkers.md)
- [ResultPattern](ResultPattern.md)
