using SystemX.SharedKernel.Domain;

namespace SystemX.SharedKernel.Time;

/// <summary>An inclusive range of dates.</summary>
public sealed class DateRange : ValueObject
{
    /// <summary>Creates a range. Start must not be after end.</summary>
    public DateRange(DateOnly start, DateOnly end)
    {
        if (start > end)
        {
            throw new ArgumentException("Start date must not be after end date.", nameof(start));
        }

        Start = start;
        End = end;
    }

    /// <summary>Gets the first date (inclusive).</summary>
    public DateOnly Start { get; }

    /// <summary>Gets the last date (inclusive).</summary>
    public DateOnly End { get; }

    /// <summary>Gets the number of days, counting both ends.</summary>
    public int Days => End.DayNumber - Start.DayNumber + 1;

    /// <summary>Returns true when the date is inside the range.</summary>
    public bool Contains(DateOnly date) => date >= Start && date <= End;

    /// <summary>Returns true when both ranges share at least one date.</summary>
    public bool Overlaps(DateRange other) => Start <= other.End && other.Start <= End;

    /// <summary>Enumerates each date in the range.</summary>
    public IEnumerable<DateOnly> EachDay()
    {
        for (var d = Start; d <= End; d = d.AddDays(1))
        {
            yield return d;
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}
