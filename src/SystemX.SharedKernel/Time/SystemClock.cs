namespace SystemX.SharedKernel.Time;

/// <summary>Default clock. It wraps a <see cref="TimeProvider"/>, so tests can pass a fake provider.</summary>
public sealed class SystemClock : IClock
{
    /// <summary>Gets the shared instance that uses <see cref="TimeProvider.System"/>.</summary>
    public static readonly SystemClock Instance = new();

    private readonly TimeProvider _provider;

    /// <summary>Creates a clock with the system time provider.</summary>
    public SystemClock() : this(TimeProvider.System)
    {
    }

    /// <summary>Creates a clock with a custom time provider (for example a fake one in tests).</summary>
    public SystemClock(TimeProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <inheritdoc />
    public DateTimeOffset UtcNow => _provider.GetUtcNow();

    /// <inheritdoc />
    public DateTimeOffset Now => _provider.GetLocalNow();

    /// <inheritdoc />
    public DateTime DateTimeUtcNow => UtcNow.UtcDateTime;

    /// <inheritdoc />
    public DateTime DateTimeNow
    {
        get
        {
            var local = Now;
            return TimeZone.Equals(TimeZoneInfo.Local)
                ? DateTime.SpecifyKind(local.DateTime, DateTimeKind.Local)
                : local.DateTime;
        }
    }

    /// <inheritdoc />
    public DateOnly UtcToday => DateOnly.FromDateTime(DateTimeUtcNow);

    /// <inheritdoc />
    public DateOnly Today => DateOnly.FromDateTime(Now.DateTime);

    /// <inheritdoc />
    public TimeOnly UtcTimeOfDay => TimeOnly.FromDateTime(DateTimeUtcNow);

    /// <inheritdoc />
    public TimeOnly TimeOfDay => TimeOnly.FromDateTime(Now.DateTime);

    /// <inheritdoc />
    public long UnixTimeSeconds => UtcNow.ToUnixTimeSeconds();

    /// <inheritdoc />
    public long UnixTimeMilliseconds => UtcNow.ToUnixTimeMilliseconds();

    /// <inheritdoc />
    public TimeZoneInfo TimeZone => _provider.LocalTimeZone;

    /// <inheritdoc />
    public long GetTimestamp() => _provider.GetTimestamp();

    /// <inheritdoc />
    public TimeSpan GetElapsedTime(long startingTimestamp) => _provider.GetElapsedTime(startingTimestamp);

    /// <inheritdoc />
    public DateTimeOffset GetNowIn(TimeZoneInfo timeZone) =>
        TimeZoneInfo.ConvertTime(UtcNow, timeZone ?? throw new ArgumentNullException(nameof(timeZone)));

    /// <inheritdoc />
    public DateOnly GetTodayIn(TimeZoneInfo timeZone) => DateOnly.FromDateTime(GetNowIn(timeZone).DateTime);
}
