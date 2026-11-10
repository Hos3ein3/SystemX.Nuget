namespace SystemX.Application.Abstractions.Cqrs;

/// <summary>
/// Marker interface for commands returning a result.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public interface ICommand<out TResult>;

/// <summary>
/// Marker interface for commands without a payload result.
/// </summary>
public interface ICommand : ICommand<SharedKernel.Results.Result>;
