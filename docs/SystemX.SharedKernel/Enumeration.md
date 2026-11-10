# Enumeration

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Domain`

## What it is
`Enumeration<TEnum>` is an abstract base class for type-safe, object-oriented enumerations that can encapsulate rich domain logic and polymorphic behavior.

## When to use it
- Use it instead of C# `enum` when enum values have behavior, display properties, or calculation logic.
- Use it when you need type safety, clean mapping, or database persistence without fragile integer casts.
- Do NOT use it for trivial internal bit flags; standard C# enums are sufficient for simple flag bitmasks.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Inherit from `Enumeration<TEnum>` and define `public static readonly` instances for each supported value.

## Sample code
```csharp
using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Domain;

namespace Example;

public abstract class OrderStatus : Enumeration<OrderStatus>
{
    public static readonly OrderStatus Submitted = new SubmittedStatus();
    public static readonly OrderStatus Processing = new ProcessingStatus();
    public static readonly OrderStatus Completed = new CompletedStatus();
    public static readonly OrderStatus Cancelled = new CancelledStatus();

    private OrderStatus(int value, string name) : base(value, name) { }

    public abstract bool CanCancel();

    private sealed class SubmittedStatus : OrderStatus
    {
        public SubmittedStatus() : base(1, nameof(Submitted)) { }
        public override bool CanCancel() => true;
    }

    private sealed class ProcessingStatus : OrderStatus
    {
        public ProcessingStatus() : base(2, nameof(Processing)) { }
        public override bool CanCancel() => true;
    }

    private sealed class CompletedStatus : OrderStatus
    {
        public CompletedStatus() : base(3, nameof(Completed)) { }
        public override bool CanCancel() => false;
    }

    private sealed class CancelledStatus : OrderStatus
    {
        public CancelledStatus() : base(4, nameof(Cancelled)) { }
        public override bool CanCancel() => false;
    }
}
```

## Configuration (if any)
No external configuration required.

## Related
- [ValueObject.md](ValueObject.md)
