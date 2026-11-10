namespace SystemX.Application.Abstractions.Cqrs;

/// <summary>
/// Marker interface for queries returning a result.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public interface IQuery<out TResult>;
