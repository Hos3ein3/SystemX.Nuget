namespace SystemX.SharedKernel.Time;

/// <summary>Abstraction of the current time. Use it instead of DateTime.UtcNow so code can be tested.</summary>
public interface IClock
{
    /// <summary>Gets the current UTC time.</summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>Gets the current time in the clock's local time zone, with its offset.</summary>
    DateTimeOffset Now { get; }

    /// <summary>Gets the current UTC time as a <see cref="DateTime"/> (Kind = Utc).</summary>
    DateTime DateTimeUtcNow { get; }

    /// <summary>Gets the current local time as a <see cref="DateTime"/>.</summary>
    DateTime DateTimeNow { get; }

    /// <summary>Gets today's date in UTC.</summary>
    DateOnly UtcToday { get; }

    /// <summary>Gets today's date in the clock's local time zone.</summary>
    DateOnly Today { get; }

    /// <summary>Gets the current UTC time of day.</summary>
    TimeOnly UtcTimeOfDay { get; }

    /// <summary>Gets the current local time of day.</summary>
    TimeOnly TimeOfDay { get; }

    /// <summary>Gets the current Unix time in seconds (UTC).</summary>
    long UnixTimeSeconds { get; }

    /// <summary>Gets the current Unix time in milliseconds (UTC).</summary>
    long UnixTimeMilliseconds { get; }

    /// <summary>Gets the local time zone of this clock.</summary>
    TimeZoneInfo TimeZone { get; }

    /// <summary>Gets a high-resolution timestamp for measuring durations. Not a date.</summary>
    long GetTimestamp();

    /// <summary>Gets the elapsed time since a timestamp from <see cref="GetTimestamp"/>.</summary>
    TimeSpan GetElapsedTime(long startingTimestamp);

    /// <summary>Gets the current time in another time zone.</summary>
    DateTimeOffset GetNowIn(TimeZoneInfo timeZone);

    /// <summary>Gets today's date in another time zone.</summary>
    DateOnly GetTodayIn(TimeZoneInfo timeZone);
}
