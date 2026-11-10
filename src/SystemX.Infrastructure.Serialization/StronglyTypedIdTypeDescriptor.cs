using System.ComponentModel;

namespace SystemX.Infrastructure.Serialization;

/// <summary>
/// Provides registration helpers for associating <see cref="StronglyTypedIdTypeConverter"/> with strongly-typed identifier types.
/// </summary>
public static class StronglyTypedIdTypeDescriptor
{
    /// <summary>
    /// Registers the <see cref="StronglyTypedIdTypeConverter"/> for the given strongly-typed identifier type.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    public static void Register<TId>()
    {
        TypeDescriptor.AddAttributes(typeof(TId), new TypeConverterAttribute(typeof(StronglyTypedIdTypeConverter)));
    }

    /// <summary>
    /// Registers the <see cref="StronglyTypedIdTypeConverter"/> for the specified runtime ID type.
    /// </summary>
    /// <param name="idType">The strongly-typed identifier type.</param>
    public static void Register(Type idType)
    {
        ArgumentNullException.ThrowIfNull(idType);
        TypeDescriptor.AddAttributes(idType, new TypeConverterAttribute(typeof(StronglyTypedIdTypeConverter)));
    }
}
