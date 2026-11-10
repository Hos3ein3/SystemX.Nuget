using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SystemX.Application.Abstractions.Cqrs;
using SystemX.Application.Abstractions.Validation;
using SystemX.SharedKernel.Results;

namespace SystemX.Infrastructure.Validation;

/// <summary>
/// Provides extension methods for registering validation services and decorators into the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="IValidationService"/> and configured <see cref="ValidationOptions"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action for validation options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSystemXValidation(
        this IServiceCollection services,
        Action<ValidationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        ValidationOptions options = new();
        configure?.Invoke(options);
        services.TryAddSingleton(options);
        services.AddScoped<IValidationService, ValidationService>();

        return services;
    }

    /// <summary>
    /// Registers FluentValidation validators from the specified assemblies, registers <see cref="IValidationService"/>,
    /// and decorates existing CQRS command and query handlers with validation logic.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for validators.</param>
    /// <param name="configure">Optional configuration action for validation options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSystemXValidation(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<ValidationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        services.AddSystemXValidation(configure);

        // Register validators from assemblies
        foreach (Assembly assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Scoped);
        }

        // Wrap existing ICommandHandler<,> and IQueryHandler<,> registrations if present
        DecorateHandlers(services, typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerDecorator<,>));
        DecorateHandlers(services, typeof(IQueryHandler<,>), typeof(ValidationQueryHandlerDecorator<,>));

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
    IValidationService validationService) : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var failures = await validationService.GetFailuresAsync(command, cancellationToken).ConfigureAwait(false);
        if (failures.Count > 0)
        {
            ValidationError validationError = new(failures);

            if (ValidationFailureResultHelper<TResult>.IsResultType)
            {
                return ValidationFailureResultHelper<TResult>.CreateFailureResult(validationError);
            }

            throw new RequestValidationException(failures);
        }

        return await innerHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
    }
}

internal sealed class ValidationQueryHandlerDecorator<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> innerHandler,
    IValidationService validationService) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        var failures = await validationService.GetFailuresAsync(query, cancellationToken).ConfigureAwait(false);
        if (failures.Count > 0)
        {
            ValidationError validationError = new(failures);

            if (ValidationFailureResultHelper<TResult>.IsResultType)
            {
                return ValidationFailureResultHelper<TResult>.CreateFailureResult(validationError);
            }

            throw new RequestValidationException(failures);
        }

        return await innerHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
    }
}
