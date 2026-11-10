namespace SystemX.SharedKernel.Time;

/// <summary>Helper methods for <see cref="IClock"/> and date/time values.</summary>
public static class ClockExtensions
{
    /// <summary>Returns true when the time is before the clock's current UTC time.</summary>
    public static bool IsPast(this IClock clock, DateTimeOffset value) => value < clock.UtcNow;

    /// <summary>Returns true when the time is after the clock's current UTC time.</summary>
    public static bool IsFuture(this IClock clock, DateTimeOffset value) => value > clock.UtcNow;

    /// <summary>Returns true when the date is before today (UTC).</summary>
    public static bool IsPast(this IClock clock, DateOnly value) => value < clock.UtcToday;

    /// <summary>Returns true when the date is after today (UTC).</summary>
    public static bool IsFuture(this IClock clock, DateOnly value) => value > clock.UtcToday;

    /// <summary>Gets the current time in the time zone with the given id (IANA or Windows id).</summary>
    public static DateTimeOffset GetNowIn(this IClock clock, string timeZoneId) =>
        clock.GetNowIn(TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));

    /// <summary>Gets today's date in the time zone with the given id.</summary>
    public static DateOnly GetTodayIn(this IClock clock, string timeZoneId) =>
        clock.GetTodayIn(TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));

    /// <summary>Converts a UTC time to another time zone.</summary>
    public static DateTimeOffset ToTimeZone(this DateTimeOffset value, TimeZoneInfo timeZone) =>
        TimeZoneInfo.ConvertTime(value, timeZone);

    /// <summary>Converts a time to a date in the given time zone.</summary>
    public static DateOnly ToDateOnly(this DateTimeOffset value, TimeZoneInfo timeZone) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(value, timeZone).DateTime);

    /// <summary>Gets the start of a date in a time zone, as a UTC instant.</summary>
    public static DateTimeOffset StartOfDayUtc(this DateOnly date, TimeZoneInfo timeZone)
    {
        var local = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        return new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(local, timeZone), TimeSpan.Zero);
    }

    /// <summary>Gets the start of the next day in a time zone, as a UTC instant. Use it as an exclusive end.</summary>
    public static DateTimeOffset EndOfDayExclusiveUtc(this DateOnly date, TimeZoneInfo timeZone) =>
        date.AddDays(1).StartOfDayUtc(timeZone);

    /// <summary>Converts a Unix time in seconds to a UTC time.</summary>
    public static DateTimeOffset FromUnixSeconds(long seconds) => DateTimeOffset.FromUnixTimeSeconds(seconds);

    /// <summary>Converts a Unix time in milliseconds to a UTC time.</summary>
    public static DateTimeOffset FromUnixMilliseconds(long milliseconds) =>
        DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);

    /// <summary>Starts a stopwatch-like measurement.</summary>
    public static ClockTimer StartTimer(this IClock clock) => new(clock);
}

/// <summary>Measures elapsed time with an <see cref="IClock"/>.</summary>
public readonly struct ClockTimer
{
    private readonly IClock _clock;
    private readonly long _start;

    /// <summary>Starts the timer.</summary>
    public ClockTimer(IClock clock)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _start = clock.GetTimestamp();
    }

    /// <summary>Gets the elapsed time since the timer started.</summary>
    public TimeSpan Elapsed => _clock.GetElapsedTime(_start);
}
