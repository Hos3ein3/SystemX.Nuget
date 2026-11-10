namespace SystemX.Application.Abstractions.BackgroundJobs;

using System;

/// <summary>
/// Represents the schedule of a recurring job: a standard 5-field cron expression or a fixed interval.
/// </summary>
public sealed record RecurringSchedule
{
    /// <summary>
    /// Gets the 5-field cron expression, or <see langword="null"/> if using a fixed interval.
    /// </summary>
    public string? Cron { get; private init; }

    /// <summary>
    /// Gets the fixed interval, or <see langword="null"/> if using a cron expression.
    /// </summary>
    public TimeSpan? Interval { get; private init; }

    /// <summary>
    /// Creates a schedule from a standard 5-field cron expression (minute hour day-of-month month day-of-week).
    /// </summary>
    /// <param name="cron">The 5-field cron expression string.</param>
    /// <returns>A new <see cref="RecurringSchedule"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the cron expression does not contain exactly 5 fields.</exception>
    public static RecurringSchedule FromCron(string cron)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cron);
        var trimmed = cron.Trim();
        var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 5)
        {
            throw new ArgumentException(
                "Cron expression must have exactly 5 fields (minute hour day-of-month month day-of-week).",
                nameof(cron));
        }

        return new RecurringSchedule { Cron = trimmed };
    }

    /// <summary>
    /// Creates a schedule repeating at the specified fixed interval.
    /// </summary>
    /// <param name="interval">The duration between job runs. Must be greater than zero.</param>
    /// <returns>A new <see cref="RecurringSchedule"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is less than or equal to zero.</exception>
    public static RecurringSchedule Every(TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be greater than zero.");
        }

        return new RecurringSchedule { Interval = interval };
    }

    /// <summary>
    /// Creates a daily schedule running at the specified time of day.
    /// </summary>
    /// <param name="at">The time of day to trigger the job.</param>
    /// <returns>A new <see cref="RecurringSchedule"/> instance.</returns>
    public static RecurringSchedule Daily(TimeOnly at) =>
        FromCron($"{at.Minute} {at.Hour} * * *");

    /// <summary>
    /// Creates a weekly schedule running on the specified day of week at the given time.
    /// </summary>
    /// <param name="day">The day of the week (Sunday = 0).</param>
    /// <param name="at">The time of day to trigger the job.</param>
    /// <returns>A new <see cref="RecurringSchedule"/> instance.</returns>
    public static RecurringSchedule Weekly(DayOfWeek day, TimeOnly at) =>
        FromCron($"{at.Minute} {at.Hour} * * {(int)day}");

    /// <summary>
    /// Creates a monthly schedule running on the specified day of the month at the given time.
    /// </summary>
    /// <param name="dayOfMonth">The day of the month (1 through 28 only to guarantee execution in February).</param>
    /// <param name="at">The time of day to trigger the job.</param>
    /// <returns>A new <see cref="RecurringSchedule"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="dayOfMonth"/> is not between 1 and 28.</exception>
    public static RecurringSchedule Monthly(int dayOfMonth, TimeOnly at)
    {
        if (dayOfMonth is < 1 or > 28)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dayOfMonth),
                "dayOfMonth must be between 1 and 28 to guarantee execution across all calendar months including February.");
        }

        return FromCron($"{at.Minute} {at.Hour} {dayOfMonth} * *");
    }
}
