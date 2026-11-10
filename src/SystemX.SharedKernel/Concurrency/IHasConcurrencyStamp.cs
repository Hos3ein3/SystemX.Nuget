namespace SystemX.SharedKernel.Concurrency;

/// <summary>
/// Marks an entity that uses an opaque concurrency stamp replaced on each update.
/// Highly portable and API-friendly (suitable for HTTP ETag headers).
/// </summary>
public interface IHasConcurrencyStamp : IConcurrencySafe
{
    /// <summary>
    /// Gets the current concurrency stamp string.
    /// </summary>
    string ConcurrencyStamp { get; }

    /// <summary>
    /// Renews the concurrency stamp with a newly generated token. Called by infrastructure interceptors upon modification.
    /// </summary>
    void RenewConcurrencyStamp();
}
