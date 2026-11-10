namespace SystemX.Infrastructure.BackgroundJobs.Core;

using System;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.BackgroundJobs;

/// <summary>
/// Represents metadata and invocation delegate for a registered background job.
/// </summary>
internal sealed record JobRegistration(
    string Name,
    Type ArgsType,
    Type JobType,
    Func<IServiceProvider, object, BackgroundJobContext, CancellationToken, Task> Invoke);
