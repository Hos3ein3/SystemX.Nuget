namespace SystemX.SharedKernel.Time;

/// <summary>Default clock that uses the system time.</summary>
public sealed class SystemClock : IClock
{
    /// <summary>Gets the shared instance.</summary>
    public static readonly SystemClock Instance = new();

    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
