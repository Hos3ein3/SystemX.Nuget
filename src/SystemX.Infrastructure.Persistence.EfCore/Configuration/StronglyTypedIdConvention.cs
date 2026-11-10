using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace SystemX.Infrastructure.Persistence.EfCore.Configuration;

/// <summary>
/// EF Core convention that automatically registers value converters for strongly-typed identifiers
/// and configures Guid/string primary keys as <see cref="ValueGenerated.Never"/>.
/// </summary>
public sealed class StronglyTypedIdConvention : IModelFinalizingConvention
{
    /// <inheritdoc />
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            foreach (var property in entityType.GetDeclaredProperties())
            {
                if (property.GetValueConverter() is not null)
                {
                    continue;
                }

                var converter = StronglyTypedIdConverters.TryCreate(property.ClrType);
                if (converter is null)
                {
                    continue;
                }

                property.SetValueConverter(converter);

                // Guid and string IDs are created by the domain, never by the database
                if (property.IsPrimaryKey() &&
                    converter.ProviderClrType != typeof(int) &&
                    converter.ProviderClrType != typeof(long))
                {
                    property.SetValueGenerated(ValueGenerated.Never);
                }
            }
        }
    }
}
