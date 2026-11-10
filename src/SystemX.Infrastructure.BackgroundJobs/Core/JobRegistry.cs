namespace SystemX.Infrastructure.BackgroundJobs.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SystemX.Application.Abstractions.BackgroundJobs;

/// <summary>
/// Maintains mappings between job arguments types, logical job names, and executable job handlers.
/// </summary>
internal sealed class JobRegistry
{
    private readonly Dictionary<Type, JobRegistration> _byArgs = new();
    private readonly Dictionary<string, JobRegistration> _byName = new(StringComparer.Ordinal);

    public void Register<TJob, TArgs>(string? nameOverride = null)
        where TJob : class, IBackgroundJob<TArgs>
        where TArgs : class
    {
        Register(typeof(TJob), typeof(TArgs), nameOverride);
    }

    public void Register(Type jobType, Type argsType, string? nameOverride = null)
    {
        ArgumentNullException.ThrowIfNull(jobType);
        ArgumentNullException.ThrowIfNull(argsType);

        var name = nameOverride
            ?? argsType.GetCustomAttribute<BackgroundJobNameAttribute>()?.Name
            ?? argsType.FullName
            ?? throw new InvalidOperationException($"Cannot determine a job name for type '{argsType}'.");

        if (_byArgs.TryGetValue(argsType, out var existingByArgs))
        {
            throw new InvalidOperationException(
                $"Duplicate job registration for args type '{argsType.FullName}'. Already registered by '{existingByArgs.JobType.FullName}', attempted by '{jobType.FullName}'.");
        }

        if (_byName.TryGetValue(name, out var existingByName))
        {
            throw new InvalidOperationException(
                $"Duplicate job registration for job name '{name}'. Already registered by '{existingByName.JobType.FullName}', attempted by '{jobType.FullName}'.");
        }

        var invoker = CreateInvoker(jobType, argsType);
        var registration = new JobRegistration(name, argsType, jobType, invoker);

        _byArgs[argsType] = registration;
        _byName[name] = registration;
    }

    public JobRegistration GetByArgs(Type argsType)
    {
        ArgumentNullException.ThrowIfNull(argsType);
        if (!_byArgs.TryGetValue(argsType, out var registration))
        {
            throw new InvalidOperationException(
                $"No job is registered for args type '{argsType.FullName}'. Register it with AddJob<TJob, TArgs>().");
        }

        return registration;
    }

    public bool TryGetByName(string name, [NotNullWhen(true)] out JobRegistration? registration) =>
        _byName.TryGetValue(name, out registration);

    public IReadOnlyCollection<JobRegistration> GetAll() => _byName.Values;

    private static Func<IServiceProvider, object, BackgroundJobContext, CancellationToken, Task> CreateInvoker(Type jobType, Type argsType)
    {
        var interfaceType = typeof(IBackgroundJob<>).MakeGenericType(argsType);
        var method = interfaceType.GetMethod(nameof(IBackgroundJob<object>.ExecuteAsync), [argsType, typeof(BackgroundJobContext), typeof(CancellationToken)])
            ?? throw new InvalidOperationException($"Method ExecuteAsync not found on {interfaceType}.");

        return (sp, args, ctx, ct) =>
        {
            var jobInstance = sp.GetRequiredService(jobType);
            var result = method.Invoke(jobInstance, [args, ctx, ct]);
            return (Task)result!;
        };
    }
}
