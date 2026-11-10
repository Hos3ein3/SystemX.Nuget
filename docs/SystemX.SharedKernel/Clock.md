# Clock and Time Primitives

**Package:** `SystemX.SharedKernel`
**Namespace:** `SystemX.SharedKernel.Time`

## What it is
`IClock`, `SystemClock`, `ClockExtensions`, `TimeInterval`, and `DateRange` provide testable, deterministic time abstractions and value objects for measuring and bounding time across domain and application logic without relying on static system time calls.

## When to use it
- Use `IClock` whenever the current UTC or local date/time, timestamp, or elapsed time is needed in domain services, command handlers, or entities.
- Use `ClockExtensions` for time zone conversions, date boundary calculations (`StartOfDayUtc`, `EndOfDayExclusiveUtc`), past/future checks, and stopwatch-style elapsed measurements via `ClockTimer`.
- Use `TimeInterval` to model half-open ranges of instants `[Start, End)` with duration, containment, and overlap detection.
- Use `DateRange` to model inclusive calendar day ranges `[Start, End]` with day count, day iteration, containment, and overlap checks.
- Do NOT use direct calls to `DateTime.Now`, `DateTime.UtcNow`, `DateTimeOffset.UtcNow`, or `Stopwatch.StartNew()` in business logic.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
1. Inject `IClock` into your services or aggregates.
2. In production, register `SystemClock.Instance` (or a `SystemClock` wrapping a custom `TimeProvider`) as a singleton.
3. In tests, supply a fake or mock `IClock` (or pass Microsoft's `FakeTimeProvider` into `new SystemClock(fakeProvider)`).
4. Use `TimeInterval` and `DateRange` as strongly-typed, immutable value objects in domain models.

## Sample code

### 1. Clock and Extensions in Domain Logic
```csharp
using System;
using SystemX.SharedKernel.Time;

namespace Example.Orders;

public sealed class OrderService
{
    private readonly IClock _clock;

    public OrderService(IClock clock)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public bool IsOrderOverdue(DateTimeOffset orderDeadline)
    {
        // ClockExtensions.IsPast checks against _clock.UtcNow
        return _clock.IsPast(orderDeadline);
    }

    public TimeInterval CreateOrderWindow(TimeSpan allowedDuration)
    {
        DateTimeOffset start = _clock.UtcNow;
        DateTimeOffset end = start.Add(allowedDuration);
        return new TimeInterval(start, end);
    }

    public void ProcessWithStopwatch()
    {
        ClockTimer timer = _clock.StartTimer();

        // Perform some operation...

        TimeSpan elapsed = timer.Elapsed;
    }
}
```

### 2. TimeInterval and DateRange Value Objects
```csharp
using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Time;

namespace Example.Scheduling;

public sealed class BookingService
{
    public bool CanBookRoom(TimeInterval requestedSlot, IEnumerable<TimeInterval> existingBookings)
    {
        foreach (TimeInterval booking in existingBookings)
        {
            if (requestedSlot.Overlaps(booking))
            {
                return false;
            }
        }

        return true;
    }

    public DateRange CreateVacationPeriod(DateOnly start, DateOnly end)
    {
        var range = new DateRange(start, end);

        // Access properties and methods
        int totalDays = range.Days;
        bool includesStartDate = range.Contains(start);

        return range;
    }
}
```

### 3. Testing with a Fake TimeProvider
```csharp
using System;
using Microsoft.Extensions.Time.Testing; // from Microsoft.Extensions.TimeProvider.Testing if used in tests
using SystemX.SharedKernel.Time;

namespace Example.Tests;

public class OrderTests
{
    public void TestClock()
    {
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetUtcNow(new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero));

        IClock clock = new SystemClock(fakeTimeProvider);

        var service = new OrderService(clock);
        bool overdue = service.IsOrderOverdue(new DateTimeOffset(2025, 12, 31, 23, 59, 59, TimeSpan.Zero));
        // overdue == true
    }
}
```

## Configuration (if any)
Register `IClock` as a singleton in your DI container:
```csharp
using Microsoft.Extensions.DependencyInjection;
using SystemX.SharedKernel.Time;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTime(this IServiceCollection services)
    {
        services.AddSingleton<IClock>(SystemClock.Instance);
        return services;
    }
}
```

| Type | Default / Implementation | Description |
|---|---|---|
| `IClock` | `SystemClock.Instance` | Provides current UTC and local date/time, timestamps, and zone conversions. |
| `SystemClock` | Wraps `TimeProvider.System` | BCL-compatible `TimeProvider` adapter. Can wrap custom/fake `TimeProvider`. |
| `ClockExtensions` | Static helpers | High-level helpers (`IsPast`, `IsFuture`, `GetNowIn`, `StartOfDayUtc`, `StartTimer`). |
| `TimeInterval` | ValueObject | Half-open `[Start, End)` interval of instants in UTC. |
| `DateRange` | ValueObject | Inclusive `[Start, End]` range of calendar dates (`DateOnly`). |

## Related
- [Auditing.md](Auditing.md)
- [MultiTenancyModels.md](MultiTenancyModels.md)
- [EntityMarkers.md](EntityMarkers.md)
