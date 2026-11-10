# Clock

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Time`

## What it is
`IClock` and `SystemClock` provide an abstraction over system time (`DateTimeOffset.UtcNow`) so that time-dependent domain logic can be reliably tested without flaky tests.

## When to use it
- Use it anywhere the current UTC time is needed in domain services, handlers, or aggregates.
- Use it to inject deterministic or frozen test clocks during unit and integration testing.
- Do NOT use direct calls to `DateTime.Now`, `DateTime.UtcNow`, or `DateTimeOffset.UtcNow` in business logic.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Inject `IClock` via dependency injection, or pass `SystemClock.Instance` by default. In tests, substitute with a mock or fake implementation.

## Sample code
```csharp
using System;
using SystemX.SharedKernel.Time;

namespace Example;

public sealed class FakeClock : IClock
{
    public FakeClock(DateTimeOffset initialTime)
    {
        UtcNow = initialTime;
    }

    public DateTimeOffset UtcNow { get; set; }
}

public sealed class SubscriptionService
{
    private readonly IClock _clock;

    public SubscriptionService(IClock clock)
    {
        _clock = clock;
    }

    public DateTimeOffset CalculateExpiryDate(int days)
    {
        return _clock.UtcNow.AddDays(days);
    }
}
```

## Configuration (if any)
Register `SystemClock.Instance` as a singleton for `IClock` in DI containers:
```csharp
// Example Microsoft DI registration
// services.AddSingleton<IClock>(SystemClock.Instance);
```

## Related
- [Auditing.md](Auditing.md)
