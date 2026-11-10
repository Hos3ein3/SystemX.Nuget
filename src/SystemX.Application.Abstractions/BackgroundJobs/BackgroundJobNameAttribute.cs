namespace SystemX.Application.Abstractions.BackgroundJobs;

using System;

/// <summary>
/// Sets the stable name of a job. Put it on the args type. Renaming the type does not break stored jobs.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class BackgroundJobNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundJobNameAttribute"/> class with the specified job name.
    /// </summary>
    /// <param name="name">The stable name identifying the background job.</param>
    public BackgroundJobNameAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    /// <summary>
    /// Gets the stable name identifying the background job.
    /// </summary>
    public string Name { get; }
}
