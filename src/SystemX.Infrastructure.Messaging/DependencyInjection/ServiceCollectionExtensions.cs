using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SystemX.Application.Abstractions.Events;
using SystemX.Infrastructure.Messaging;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering integration event dispatcher and handlers.
/// </summary>
public static class IntegrationEventRegistrationExtensions
{
    /// <summary>
    /// Registers the <see cref="IIntegrationEventDispatcher"/> as a scoped service and configures its options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action for dispatcher options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddIntegrationEventDispatcher(
        this IServiceCollection services,
        Action<IntegrationEventDispatcherOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new IntegrationEventDispatcherOptions();
        configure?.Invoke(options);

        services.TryAddSingleton(options);
        services.AddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();

        return services;
    }

    /// <summary>
    /// Scans the provided assemblies for concrete, non-abstract classes implementing <see cref="IIntegrationEventHandler{T}"/>
    /// and registers each as a scoped service for every closed interface it implements.
    /// Uses <see cref="ServiceCollectionDescriptorExtensions.TryAddEnumerable(IServiceCollection, ServiceDescriptor)"/>
    /// so duplicate calls do not result in multiple registrations of the same handler.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">One or more assemblies to scan for handlers.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when no assemblies are specified.</exception>
    public static IServiceCollection AddIntegrationEventHandlers(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (assemblies == null || assemblies.Length == 0)
        {
            throw new ArgumentException("At least one assembly must be provided to register integration event handlers.", nameof(assemblies));
        }

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false });

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));

                foreach (var iface in interfaces)
                {
                    services.TryAddEnumerable(ServiceDescriptor.Scoped(iface, type));
                }
            }
        }

        return services;
    }
}
