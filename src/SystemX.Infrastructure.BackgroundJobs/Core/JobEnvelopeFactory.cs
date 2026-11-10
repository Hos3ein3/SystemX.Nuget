namespace SystemX.Infrastructure.BackgroundJobs.Core;

using System;
using System.Diagnostics;
using System.Text.Json;
using SystemX.Application.Abstractions.Localization;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Application.Abstractions.Security;
using SystemX.SharedKernel.Time;

/// <summary>
/// Factory service responsible for creating <see cref="JobEnvelope"/> instances while capturing ambient execution context.
/// </summary>
internal sealed class JobEnvelopeFactory
{
    private readonly JobRegistry _registry;
    private readonly BackgroundJobsOptions _options;
    private readonly IClock _clock;
    private readonly ITenantContext? _tenantContext;
    private readonly ICurrentCulture? _currentCulture;
    private readonly ICurrentUser? _currentUser;

    public JobEnvelopeFactory(
        JobRegistry registry,
        BackgroundJobsOptions options,
        IClock clock,
        ITenantContext? tenantContext = null,
        ICurrentCulture? currentCulture = null,
        ICurrentUser? currentUser = null)
    {
        _registry = registry;
        _options = options;
        _clock = clock;
        _tenantContext = tenantContext;
        _currentCulture = currentCulture;
        _currentUser = currentUser;
    }

    public JobEnvelope Create<TArgs>(
        TArgs args,
        bool propagateTenant,
        bool propagateCulture,
        string? correlationId,
        int? maxRetries,
        bool allowConcurrentExecution) where TArgs : class
    {
        ArgumentNullException.ThrowIfNull(args);

        var registration = _registry.GetByArgs(typeof(TArgs));
        var argsJson = JsonSerializer.Serialize(args, typeof(TArgs), _options.JsonSerializerOptions);

        var tenantId = propagateTenant ? _tenantContext?.TenantId?.Value : null;
        var culture = propagateCulture ? _currentCulture?.Name : null;
        var corrId = correlationId ?? Activity.Current?.Id;
        var userId = _currentUser?.UserId;
        var retries = maxRetries ?? _options.DefaultMaxRetries;
        var enqueuedAt = _clock.UtcNow;

        return new JobEnvelope(
            registration.Name,
            argsJson,
            tenantId,
            culture,
            corrId,
            userId,
            enqueuedAt,
            retries,
            allowConcurrentExecution);
    }
}
