namespace SystemX.Application.Abstractions.BackgroundJobs;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a background job. The arguments type identifies the job; register exactly one job class per arguments type.
/// </summary>
/// <typeparam name="TArgs">The type of the arguments passed to the job.</typeparam>
public interface IBackgroundJob<in TArgs> where TArgs : class
{
    /// <summary>
    /// Executes the background job with the provided arguments and context.
    /// </summary>
    /// <param name="args">The job input arguments.</param>
    /// <param name="context">The contextual metadata for this execution.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous execution operation.</returns>
    Task ExecuteAsync(TArgs args, BackgroundJobContext context, CancellationToken cancellationToken);
}
