using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SystemX.Application.Abstractions.Validation;
using SystemX.SharedKernel.Results;
using SharedValidationFailure = SystemX.SharedKernel.Results.ValidationFailure;

namespace SystemX.Infrastructure.Validation;

/// <summary>
/// Default implementation of <see cref="IValidationService"/> using FluentValidation validators.
/// </summary>
internal sealed class ValidationService : IValidationService
{
    private static readonly ConcurrentDictionary<Type, Func<ValidationService, object, CancellationToken, Task<Result>>> ObjectInvokers = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly ValidationOptions _options;

    public ValidationService(IServiceProvider serviceProvider, ValidationOptions options)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<Result> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default) where T : notnull
    {
        var failures = await GetFailuresAsync(instance, cancellationToken).ConfigureAwait(false);
        return failures.Count == 0 ? Result.Success() : Result.Failure(Error.Validation(failures));
    }

    public async Task EnsureValidAsync<T>(T instance, CancellationToken cancellationToken = default) where T : notnull
    {
        var failures = await GetFailuresAsync(instance, cancellationToken).ConfigureAwait(false);
        if (failures.Count > 0)
        {
            throw new RequestValidationException(failures);
        }
    }

    public async Task<IReadOnlyList<SharedValidationFailure>> GetFailuresAsync<T>(
        T instance, CancellationToken cancellationToken = default) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(instance);

        var validators = _serviceProvider.GetServices<IValidator<T>>().ToList();
        if (validators.Count == 0)
        {
            return Array.Empty<SharedValidationFailure>();
        }

        var context = new ValidationContext<T>(instance);
        var failures = new List<SharedValidationFailure>();

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
            if (result.IsValid)
            {
                continue;
            }

            failures.AddRange(result.ToFailures(_options.IncludeErrorCodes));
            if (_options.StopOnFirstFailure)
            {
                break;
            }
        }

        return failures;
    }

    public Task<Result> ValidateObjectAsync(object instance, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instance);

        var invoker = ObjectInvokers.GetOrAdd(instance.GetType(), static type =>
        {
            var method = typeof(ValidationService)
                .GetMethod(nameof(ValidateAsync), BindingFlags.Public | BindingFlags.Instance)!
                .MakeGenericMethod(type);

            return (service, obj, ct) => (Task<Result>)method.Invoke(service, [obj, ct])!;
        });

        return invoker(this, instance, cancellationToken);
    }
}
