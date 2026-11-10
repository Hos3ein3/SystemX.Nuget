namespace SystemX.Application.Abstractions.Security;

/// <summary>
/// Provides access to the identity information of the currently authenticated user in the execution context.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets a value indicating whether a user is authenticated in the current context.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the unique identifier of the authenticated user, or <see langword="null"/> if unauthenticated.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the username or display name of the authenticated user, or <see langword="null"/> if unauthenticated.
    /// </summary>
    string? UserName { get; }
}
