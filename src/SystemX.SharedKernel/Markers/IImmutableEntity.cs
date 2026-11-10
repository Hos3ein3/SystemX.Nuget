namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Marks an immutable entity (e.g. audit logs, ledger entries, immutable domain events).
/// Persistence layers reject updates and deletions to entities implementing this interface.
/// </summary>
public interface IImmutableEntity
{
}
