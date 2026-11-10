namespace SystemX.SharedKernel.Auditing;

/// <summary>An object that tracks who created and changed it, and when.</summary>
public interface IAuditable
{
    /// <summary>Gets the UTC creation time.</summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>Gets the creator id or name.</summary>
    string? CreatedBy { get; }

    /// <summary>Gets the UTC time of the last change.</summary>
    DateTimeOffset? ModifiedAt { get; }

    /// <summary>Gets who made the last change.</summary>
    string? ModifiedBy { get; }

    /// <summary>Sets creation data. Called by infrastructure only.</summary>
    void SetCreated(DateTimeOffset at, string? by);

    /// <summary>Sets modification data. Called by infrastructure only.</summary>
    void SetModified(DateTimeOffset at, string? by);
}
