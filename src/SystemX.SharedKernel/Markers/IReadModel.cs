namespace SystemX.SharedKernel.Markers;

/// <summary>
/// Marks a CQRS read-side projection model.
/// Read models are queried without change tracking, are never treated as aggregate roots, and never raise domain events.
/// </summary>
public interface IReadModel
{
}
