using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemX.SharedKernel.Identifiers;

namespace SystemX.Infrastructure.Serialization;

/// <summary>
/// JSON converter factory for automatically resolving and constructing converters for types deriving from <see cref="StronglyTypedId{TValue}"/>.
/// </summary>
public sealed class StronglyTypedIdJsonConverterFactory : JsonConverterFactory
{
    private static readonly ConcurrentDictionary<Type, JsonConverter> ConverterCache = new();
    private readonly StronglyTypedIdJsonOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="StronglyTypedIdJsonConverterFactory"/>.
    /// </summary>
    /// <param name="options">Options controlling ID serialization.</param>
    public StronglyTypedIdJsonConverterFactory(StronglyTypedIdJsonOptions? options = null)
    {
        _options = options ?? new StronglyTypedIdJsonOptions();
    }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        return GetUnderlyingIdType(typeToConvert) != null;
    }

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);

        return ConverterCache.GetOrAdd(typeToConvert, type =>
        {
            Type? baseType = GetUnderlyingIdType(type);
            if (baseType == null)
            {
                throw new InvalidOperationException($"Type {type.Name} is not a strongly-typed ID.");
            }

            Type valueType = baseType.GetGenericArguments()[0];

            // Resolve factory: check if type implements IGuidIdFactory, IIntIdFactory, etc., or has a constructor with (valueType)
            Delegate factoryDelegate = ResolveFactoryDelegate(type, valueType);

            Type converterType = typeof(StronglyTypedIdJsonConverter<,>).MakeGenericType(type, valueType);
            return (JsonConverter)Activator.CreateInstance(converterType, factoryDelegate, _options)!;
        });
    }

    private static Type? GetUnderlyingIdType(Type type)
    {
        Type? current = type;
        while (current != null && current != typeof(object))
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(StronglyTypedId<>))
            {
                return current;
            }

            current = current.BaseType;
        }

        return null;
    }

    private static Delegate ResolveFactoryDelegate(Type idType, Type valueType)
    {
        // 1. Look for static Create(valueType) method
        MethodInfo? createMethod = idType.GetMethod(
            "Create",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: [valueType],
            modifiers: null);

        if (createMethod != null)
        {
            Type funcType = typeof(Func<,>).MakeGenericType(valueType, idType);
            return Delegate.CreateDelegate(funcType, createMethod);
        }

        // 2. Look for public constructor with single parameter of valueType
        ConstructorInfo? ctor = idType.GetConstructor([valueType]);
        if (ctor != null)
        {
            Type funcType = typeof(Func<,>).MakeGenericType(valueType, idType);
            return (object val) => ctor.Invoke([val]);
        }

        throw new InvalidOperationException(
            $"Cannot resolve factory for strongly typed ID '{idType.Name}'. " +
            $"Ensure it implements a static Create({valueType.Name}) method or provides a public constructor accepting {valueType.Name}.");
    }
}
