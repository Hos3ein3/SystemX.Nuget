using SystemX.SharedKernel.Domain;

namespace SystemX.SharedKernel.Time;

/// <summary>A half-open interval of instants: Start is included, End is excluded.</summary>
public sealed class TimeInterval : ValueObject
{
    /// <summary>Creates an interval. Start must be before end.</summary>
    public TimeInterval(DateTimeOffset start, DateTimeOffset end)
    {
        if (start >= end)
        {
            throw new ArgumentException("Start must be before end.", nameof(start));
        }

        Start = start.ToUniversalTime();
        End = end.ToUniversalTime();
    }

    /// <summary>Gets the start (inclusive, UTC).</summary>
    public DateTimeOffset Start { get; }

    /// <summary>Gets the end (exclusive, UTC).</summary>
    public DateTimeOffset End { get; }

    /// <summary>Gets the length.</summary>
    public TimeSpan Duration => End - Start;

    /// <summary>Returns true when the instant is inside the interval.</summary>
    public bool Contains(DateTimeOffset instant) => instant >= Start && instant < End;

    /// <summary>Returns true when both intervals share at least one instant.</summary>
    public bool Overlaps(TimeInterval other) => Start < other.End && other.Start < End;

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}
