using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SystemX.Infrastructure.Serialization;

/// <summary>
/// Extension methods for configuring SystemX JSON serialization converters and DI registrations.
/// </summary>
public static class SerializationExtensions
{
    /// <summary>
    /// Adds SystemX JSON converters (including strongly-typed ID support) to the specified <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <param name="options">The JSON serializer options to configure.</param>
    /// <param name="stronglyTypedIdOptions">Optional settings for strongly-typed ID serialization.</param>
    /// <returns>The configured options for chaining.</returns>
    public static JsonSerializerOptions AddSystemXConverters(
        this JsonSerializerOptions options,
        StronglyTypedIdJsonOptions? stronglyTypedIdOptions = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Converters.Add(new StronglyTypedIdJsonConverterFactory(stronglyTypedIdOptions));
        return options;
    }

    /// <summary>
    /// Registers SystemX serialization options and configures named <see cref="JsonSerializerOptions"/> in the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration for strongly-typed ID serialization options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSystemXSerialization(
        this IServiceCollection services,
        Action<StronglyTypedIdJsonOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.AddOptions<StronglyTypedIdJsonOptions>();
        }

        services.AddTransient<IConfigureOptions<JsonSerializerOptions>>(sp =>
        {
            IOptions<StronglyTypedIdJsonOptions> idOptions = sp.GetRequiredService<IOptions<StronglyTypedIdJsonOptions>>();
            return new ConfigureNamedOptions<JsonSerializerOptions>(Options.DefaultName, opt =>
            {
                opt.AddSystemXConverters(idOptions.Value);
            });
        });

        return services;
    }
}
