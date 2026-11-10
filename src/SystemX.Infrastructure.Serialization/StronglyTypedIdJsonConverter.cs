using System.Text.Json;
using System.Text.Json.Serialization;
using SystemX.SharedKernel.Identifiers;

namespace SystemX.Infrastructure.Serialization;

/// <summary>
/// Custom System.Text.Json converter for strongly-typed identifiers.
/// </summary>
/// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
/// <typeparam name="TValue">The underlying primitive value type.</typeparam>
public class StronglyTypedIdJsonConverter<TId, TValue> : JsonConverter<TId>
    where TId : StronglyTypedId<TValue>
    where TValue : notnull, IComparable<TValue>
{
    private readonly Func<TValue, TId> _factory;
    private readonly StronglyTypedIdJsonOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="StronglyTypedIdJsonConverter{TId, TValue}"/>.
    /// </summary>
    /// <param name="factory">The factory function to instantiate the identifier from the primitive.</param>
    /// <param name="options">Serialization options.</param>
    public StronglyTypedIdJsonConverter(Func<TValue, TId> factory, StronglyTypedIdJsonOptions? options = null)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _options = options ?? new StronglyTypedIdJsonOptions();
    }

    /// <inheritdoc />
    public override TId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeof(TValue) == typeof(Guid))
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected JSON string at path '{reader.GetString() ?? reader.TokenType.ToString()}' for identifier {typeof(TId).Name}.");
            }

            string? stringValue = reader.GetString();
            if (!Uuid.TryParse(stringValue, out Guid guid) || guid == Guid.Empty)
            {
                throw new JsonException($"Invalid GUID string '{stringValue}' for identifier {typeof(TId).Name}.");
            }

            return _factory((TValue)(object)guid);
        }

        if (typeof(TValue) == typeof(int))
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException($"Expected JSON number for identifier {typeof(TId).Name}.");
            }

            int intValue = reader.GetInt32();
            if (intValue <= 0)
            {
                throw new JsonException($"Integer identifier {typeof(TId).Name} must be positive, got {intValue}.");
            }

            return _factory((TValue)(object)intValue);
        }

        if (typeof(TValue) == typeof(long))
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException($"Expected JSON number for identifier {typeof(TId).Name}.");
            }

            long longValue = reader.GetInt64();
            if (longValue <= 0)
            {
                throw new JsonException($"Long identifier {typeof(TId).Name} must be positive, got {longValue}.");
            }

            return _factory((TValue)(object)longValue);
        }

        if (typeof(TValue) == typeof(string))
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected JSON string for identifier {typeof(TId).Name}.");
            }

            string? strValue = reader.GetString();
            if (string.IsNullOrWhiteSpace(strValue))
            {
                throw new JsonException($"String identifier {typeof(TId).Name} cannot be null or whitespace.");
            }

            return _factory((TValue)(object)strValue);
        }

        throw new JsonException($"Unsupported underlying value type {typeof(TValue).Name} for {typeof(TId).Name}.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);

        if (value.Value is Guid guid)
        {
            writer.WriteStringValue(Uuid.ToString(guid, _options.WriteFormat));
        }
        else if (value.Value is int intVal)
        {
            writer.WriteNumberValue(intVal);
        }
        else if (value.Value is long longVal)
        {
            writer.WriteNumberValue(longVal);
        }
        else if (value.Value is string strVal)
        {
            writer.WriteStringValue(strVal);
        }
        else
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    /// <inheritdoc />
    public override TId ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? propertyName = reader.GetString();
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new JsonException($"Property name for identifier {typeof(TId).Name} cannot be empty.");
        }

        if (typeof(TValue) == typeof(Guid))
        {
            if (!Uuid.TryParse(propertyName, out Guid guid) || guid == Guid.Empty)
            {
                throw new JsonException($"Invalid GUID property name '{propertyName}' for identifier {typeof(TId).Name}.");
            }

            return _factory((TValue)(object)guid);
        }

        if (typeof(TValue) == typeof(int))
        {
            if (!int.TryParse(propertyName, out int intVal) || intVal <= 0)
            {
                throw new JsonException($"Invalid positive integer property name '{propertyName}' for identifier {typeof(TId).Name}.");
            }

            return _factory((TValue)(object)intVal);
        }

        if (typeof(TValue) == typeof(long))
        {
            if (!long.TryParse(propertyName, out long longVal) || longVal <= 0)
            {
                throw new JsonException($"Invalid positive long integer property name '{propertyName}' for identifier {typeof(TId).Name}.");
            }

            return _factory((TValue)(object)longVal);
        }

        if (typeof(TValue) == typeof(string))
        {
            return _factory((TValue)(object)propertyName);
        }

        throw new JsonException($"Unsupported underlying value type {typeof(TValue).Name} for {typeof(TId).Name}.");
    }

    /// <inheritdoc />
    public override void WriteAsPropertyName(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);

        if (value.Value is Guid guid)
        {
            writer.WritePropertyName(Uuid.ToString(guid, _options.WriteFormat));
        }
        else
        {
            writer.WritePropertyName(value.ToString());
        }
    }
}
