using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SystemX.SharedKernel.Identifiers;

namespace SystemX.Infrastructure.Persistence.EfCore.Configuration;

/// <summary>
/// Provides cached EF Core value converters for strongly-typed identifiers.
/// </summary>
public static class StronglyTypedIdConverters
{
    private static readonly ConcurrentDictionary<Type, ValueConverter?> Cache = new();

    /// <summary>
    /// Attempts to create a <see cref="ValueConverter"/> for a strongly-typed ID CLR type.
    /// </summary>
    /// <param name="clrType">The CLR property type.</param>
    /// <returns>A matching value converter, or <see langword="null"/> if not applicable.</returns>
    public static ValueConverter? TryCreate(Type clrType)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        return Cache.GetOrAdd(clrType, static type =>
        {
            if (type.IsAbstract)
            {
                return null;
            }

            if (typeof(GuidId).IsAssignableFrom(type))
            {
                var factoryInterface = typeof(IGuidIdFactory<>).MakeGenericType(type);
                if (factoryInterface.IsAssignableFrom(type))
                {
                    var converterType = typeof(GuidIdValueConverter<>).MakeGenericType(type);
                    return (ValueConverter)Activator.CreateInstance(converterType)!;
                }
            }
            else if (typeof(IntId).IsAssignableFrom(type))
            {
                var factoryInterface = typeof(IIntIdFactory<>).MakeGenericType(type);
                if (factoryInterface.IsAssignableFrom(type))
                {
                    var converterType = typeof(IntIdValueConverter<>).MakeGenericType(type);
                    return (ValueConverter)Activator.CreateInstance(converterType)!;
                }
            }
            else if (typeof(LongId).IsAssignableFrom(type))
            {
                var factoryInterface = typeof(ILongIdFactory<>).MakeGenericType(type);
                if (factoryInterface.IsAssignableFrom(type))
                {
                    var converterType = typeof(LongIdValueConverter<>).MakeGenericType(type);
                    return (ValueConverter)Activator.CreateInstance(converterType)!;
                }
            }
            else if (typeof(StringId).IsAssignableFrom(type))
            {
                var factoryInterface = typeof(IStringIdFactory<>).MakeGenericType(type);
                if (factoryInterface.IsAssignableFrom(type))
                {
                    var converterType = typeof(StringIdValueConverter<>).MakeGenericType(type);
                    return (ValueConverter)Activator.CreateInstance(converterType)!;
                }
            }

            return null;
        });
    }
}

/// <summary>
/// EF Core value converter for <see cref="GuidId"/> strongly-typed identifiers.
/// </summary>
/// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
public sealed class GuidIdValueConverter<TId> : ValueConverter<TId, Guid>
    where TId : GuidId, IGuidIdFactory<TId>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GuidIdValueConverter{TId}"/>.
    /// </summary>
    public GuidIdValueConverter()
        : base(id => id.Value, value => GuidIds.From<TId>(value))
    {
    }
}

/// <summary>
/// EF Core value converter for <see cref="IntId"/> strongly-typed identifiers.
/// </summary>
/// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
public sealed class IntIdValueConverter<TId> : ValueConverter<TId, int>
    where TId : IntId, IIntIdFactory<TId>
{
    /// <summary>
    /// Initializes a new instance of <see cref="IntIdValueConverter{TId}"/>.
    /// </summary>
    public IntIdValueConverter()
        : base(id => id.Value, value => IntIds.From<TId>(value))
    {
    }
}

/// <summary>
/// EF Core value converter for <see cref="LongId"/> strongly-typed identifiers.
/// </summary>
/// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
public sealed class LongIdValueConverter<TId> : ValueConverter<TId, long>
    where TId : LongId, ILongIdFactory<TId>
{
    /// <summary>
    /// Initializes a new instance of <see cref="LongIdValueConverter{TId}"/>.
    /// </summary>
    public LongIdValueConverter()
        : base(id => id.Value, value => LongIds.From<TId>(value))
    {
    }
}

/// <summary>
/// EF Core value converter for <see cref="StringId"/> strongly-typed identifiers.
/// </summary>
/// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
public sealed class StringIdValueConverter<TId> : ValueConverter<TId, string>
    where TId : StringId, IStringIdFactory<TId>
{
    /// <summary>
    /// Initializes a new instance of <see cref="StringIdValueConverter{TId}"/>.
    /// </summary>
    public StringIdValueConverter()
        : base(id => id.Value, value => StringIds.From<TId>(value))
    {
    }
}
