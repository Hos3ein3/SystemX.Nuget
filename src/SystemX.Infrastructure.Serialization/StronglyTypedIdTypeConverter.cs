using System.ComponentModel;
using System.Globalization;
using SystemX.SharedKernel.Identifiers;

namespace SystemX.Infrastructure.Serialization;

/// <summary>
/// TypeConverter supporting conversion between primitive string representation and strongly-typed identifiers.
/// </summary>
public sealed class StronglyTypedIdTypeConverter : TypeConverter
{
    private readonly Type _idType;
    private readonly Type _valueType;
    private readonly Delegate _factory;

    /// <summary>
    /// Initializes a new instance of <see cref="StronglyTypedIdTypeConverter"/> for the given ID type.
    /// </summary>
    /// <param name="idType">The strongly-typed identifier type.</param>
    public StronglyTypedIdTypeConverter(Type idType)
    {
        _idType = idType ?? throw new ArgumentNullException(nameof(idType));

        Type? current = idType;
        Type? baseId = null;
        while (current != null && current != typeof(object))
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(StronglyTypedId<>))
            {
                baseId = current;
                break;
            }
            current = current.BaseType;
        }

        if (baseId == null)
        {
            throw new ArgumentException($"Type '{idType.Name}' does not inherit from StronglyTypedId<T>.", nameof(idType));
        }

        _valueType = baseId.GetGenericArguments()[0];

        System.Reflection.MethodInfo? createMethod = idType.GetMethod(
            "Create",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
            binder: null,
            types: [_valueType],
            modifiers: null);

        if (createMethod != null)
        {
            Type funcType = typeof(Func<,>).MakeGenericType(_valueType, idType);
            _factory = Delegate.CreateDelegate(funcType, createMethod);
        }
        else
        {
            System.Reflection.ConstructorInfo? ctor = idType.GetConstructor([_valueType]);
            if (ctor != null)
            {
                _factory = (object val) => ctor.Invoke([val]);
            }
            else
            {
                throw new InvalidOperationException($"Cannot resolve factory for ID {idType.Name}.");
            }
        }
    }

    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || sourceType == _valueType || base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc />
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || destinationType == _valueType || base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc />
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            if (_valueType == typeof(Guid))
            {
                if (Uuid.TryParse(stringValue, out Guid guid) && guid != Guid.Empty)
                {
                    return _factory.DynamicInvoke(guid);
                }
                throw new FormatException($"String '{stringValue}' is not a valid GUID for {_idType.Name}.");
            }

            if (_valueType == typeof(int))
            {
                if (int.TryParse(stringValue, NumberStyles.Integer, culture ?? CultureInfo.InvariantCulture, out int intVal) && intVal > 0)
                {
                    return _factory.DynamicInvoke(intVal);
                }
                throw new FormatException($"String '{stringValue}' is not a valid positive integer for {_idType.Name}.");
            }

            if (_valueType == typeof(long))
            {
                if (long.TryParse(stringValue, NumberStyles.Integer, culture ?? CultureInfo.InvariantCulture, out long longVal) && longVal > 0)
                {
                    return _factory.DynamicInvoke(longVal);
                }
                throw new FormatException($"String '{stringValue}' is not a valid positive 64-bit integer for {_idType.Name}.");
            }

            if (_valueType == typeof(string))
            {
                return _factory.DynamicInvoke(stringValue);
            }
        }

        if (value.GetType() == _valueType)
        {
            return _factory.DynamicInvoke(value);
        }

        return base.ConvertFrom(context, culture, value);
    }

    /// <inheritdoc />
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value == null)
        {
            return null;
        }

        if (destinationType == typeof(string))
        {
            return value.ToString();
        }

        if (destinationType == _valueType)
        {
            System.Reflection.PropertyInfo? valProp = value.GetType().GetProperty("Value");
            return valProp?.GetValue(value);
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
