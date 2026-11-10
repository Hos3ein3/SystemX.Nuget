namespace SystemX.SharedKernel.Time;

/// <summary>Abstraction of the current time. Use it instead of DateTime.UtcNow so code can be tested.</summary>
public interface IClock
{
    /// <summary>Gets the current UTC time.</summary>
    DateTimeOffset UtcNow { get; }
}
