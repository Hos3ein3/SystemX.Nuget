namespace SystemX.Application.Abstractions.Cqrs;

/// <summary>
/// Defines a handler for a command.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResult">The command result type.</typeparam>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">The command instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The command execution result.</returns>
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a handler for a parameterless result command.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, SharedKernel.Results.Result>
    where TCommand : ICommand<SharedKernel.Results.Result>;
