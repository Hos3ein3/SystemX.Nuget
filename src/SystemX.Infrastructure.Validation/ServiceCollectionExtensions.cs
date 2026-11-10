using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SystemX.Application.Abstractions.Cqrs;
using SystemX.SharedKernel.Results;

namespace SystemX.Infrastructure.Validation;

/// <summary>
/// Provides extension methods for registering validation services and decorators into the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers FluentValidation validators from the specified assemblies and decorates CQRS command and query handlers with validation logic.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for validators.</param>
    /// <param name="configure">Optional configuration action for validation options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no command or query handlers are registered prior to calling this method.</exception>
    public static IServiceCollection AddSystemXValidation(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<ValidationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.AddOptions<ValidationOptions>();
        }

        // Register validators from assemblies
        foreach (Assembly assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Scoped);
        }

        // Wrap existing ICommandHandler<,> and IQueryHandler<,> registrations
        bool decoratedAny = DecorateHandlers(services, typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerDecorator<,>));
        bool decoratedQuery = DecorateHandlers(services, typeof(IQueryHandler<,>), typeof(ValidationQueryHandlerDecorator<,>));

        if (!decoratedAny && !decoratedQuery)
        {
            throw new InvalidOperationException(
                "AddSystemXValidation was called, but no command or query handler registrations were found in the IServiceCollection. " +
                "Ensure that CQRS handlers are registered BEFORE calling AddSystemXValidation.");
        }

        return services;
    }

    private static bool DecorateHandlers(IServiceCollection services, Type handlerServiceType, Type decoratorGenericType)
    {
        bool found = false;
        List<ServiceDescriptor> descriptorsToReplace = [];

        foreach (ServiceDescriptor descriptor in services)
        {
            if (descriptor.ServiceType.IsGenericType &&
                descriptor.ServiceType.GetGenericTypeDefinition() == handlerServiceType)
            {
                descriptorsToReplace.Add(descriptor);
                found = true;
            }
        }

        foreach (ServiceDescriptor descriptor in descriptorsToReplace)
        {
            int index = services.IndexOf(descriptor);
            Type[] genericArgs = descriptor.ServiceType.GetGenericArguments();
            Type closedDecorator = decoratorGenericType.MakeGenericType(genericArgs);

            ServiceDescriptor decoratedDescriptor = ServiceDescriptor.Describe(
                descriptor.ServiceType,
                sp =>
                {
                    object innerHandler = CreateInstance(sp, descriptor);
                    return ActivatorUtilities.CreateInstance(sp, closedDecorator, innerHandler);
                },
                descriptor.Lifetime);

            services[index] = decoratedDescriptor;
        }

        return found;
    }

    private static object CreateInstance(IServiceProvider sp, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance != null)
        {
            return descriptor.ImplementationInstance;
        }

        if (descriptor.ImplementationFactory != null)
        {
            return descriptor.ImplementationFactory(sp);
        }

        return ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.ImplementationType!);
    }
}

internal static class ValidationFailureResultHelper<TResult>
{
    private static readonly Func<ValidationError, TResult>? Factory = CreateFactory();

    public static bool IsResultType => Factory != null;

    public static TResult CreateFailureResult(ValidationError error)
    {
        if (Factory == null)
        {
            throw new InvalidOperationException($"Type {typeof(TResult).Name} is not a Result type.");
        }

        return Factory(error);
    }

    private static Func<ValidationError, TResult>? CreateFactory()
    {
        Type targetType = typeof(TResult);

        if (targetType == typeof(Result))
        {
            return error => (TResult)(object)Result.Failure(error);
        }

        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = targetType.GetGenericArguments()[0];
            MethodInfo failureMethod = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethodDefinition && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(Error))
                .MakeGenericMethod(valueType);

            return error => (TResult)failureMethod.Invoke(null, [error])!;
        }

        return null;
    }
}

internal sealed class ValidationCommandHandlerDecorator<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> innerHandler,
    IEnumerable<IValidator<TCommand>> validators,
    IOptions<ValidationOptions> options) : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        if (validators.Any())
        {
            ValidationContext<TCommand> context = new(command);
            List<FluentValidation.Results.ValidationFailure> failures = [];

            foreach (IValidator<TCommand> validator in validators)
            {
                ValidationResult result = await validator.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
                if (!result.IsValid)
                {
                    failures.AddRange(result.Errors);
                    if (options.Value.StopOnFirstFailure)
                    {
                        break;
                    }
                }
            }

            if (failures.Count > 0)
            {
                List<SharedKernel.Results.ValidationFailure> domainFailures = new(failures.Count);
                foreach (FluentValidation.Results.ValidationFailure f in failures)
                {
                    string code = options.Value.IncludeErrorCodes && !string.IsNullOrWhiteSpace(f.ErrorCode)
                        ? f.ErrorCode
                        : "Validation.Failed";
                    domainFailures.Add(new SharedKernel.Results.ValidationFailure(f.PropertyName, code, f.ErrorMessage));
                }

                ValidationError validationError = new(domainFailures);

                if (ValidationFailureResultHelper<TResult>.IsResultType)
                {
                    return ValidationFailureResultHelper<TResult>.CreateFailureResult(validationError);
                }

                throw new RequestValidationException(domainFailures);
            }
        }

        return await innerHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
    }
}

internal sealed class ValidationQueryHandlerDecorator<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> innerHandler,
    IEnumerable<IValidator<TQuery>> validators,
    IOptions<ValidationOptions> options) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        if (validators.Any())
        {
            ValidationContext<TQuery> context = new(query);
            List<FluentValidation.Results.ValidationFailure> failures = [];

            foreach (IValidator<TQuery> validator in validators)
            {
                ValidationResult result = await validator.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
                if (!result.IsValid)
                {
                    failures.AddRange(result.Errors);
                    if (options.Value.StopOnFirstFailure)
                    {
                        break;
                    }
                }
            }

            if (failures.Count > 0)
            {
                List<SharedKernel.Results.ValidationFailure> domainFailures = new(failures.Count);
                foreach (FluentValidation.Results.ValidationFailure f in failures)
                {
                    string code = options.Value.IncludeErrorCodes && !string.IsNullOrWhiteSpace(f.ErrorCode)
                        ? f.ErrorCode
                        : "Validation.Failed";
                    domainFailures.Add(new SharedKernel.Results.ValidationFailure(f.PropertyName, code, f.ErrorMessage));
                }

                ValidationError validationError = new(domainFailures);

                if (ValidationFailureResultHelper<TResult>.IsResultType)
                {
                    return ValidationFailureResultHelper<TResult>.CreateFailureResult(validationError);
                }

                throw new RequestValidationException(domainFailures);
            }
        }

        return await innerHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
    }
}
