using Domain.Entities.MidjourneyVersions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Persistans.Configuration;

public class VersionsMasterConfiguration : IEntityTypeConfiguration<MidjourneyVersionsMaster>
{
    public void Configure(EntityTypeBuilder<MidjourneyVersionsMaster> builder)
    {
        builder.HasKey(version => version.Version);

        builder.Property(version => version.Version)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(version => version.ReleaseDate)
            .HasColumnType("date");

        builder.Property(version => version.Description)
            .HasColumnType("text");

        string[] collectionProps =
[
            "MidjourneyVersion1",
            "MidjourneyVersion2",
            "MidjourneyVersion3",
            "MidjourneyVersion4",
            "MidjourneyVersion5",
            "MidjourneyVersion51",
            "MidjourneyVersion52",
            "MidjourneyVersion6",
            "MidjourneyVersion61",
            "MidjourneyVersion7",
            "MidjourneyVersionNiji4",
            "MidjourneyVersionNiji5",
            "MidjourneyVersionNiji6"
        ];

        foreach (var propName in collectionProps)
        {
            var navigation = typeof(MidjourneyVersionsMaster).GetProperty(propName);
            if (navigation == null) continue;

            var targetType = navigation.PropertyType.GetGenericArguments()[0];

            var hasManyMethod = typeof(EntityTypeBuilder<MidjourneyVersionsMaster>)
                .GetMethods()
                .First(m => m.Name == "HasMany" && m.GetParameters().Length == 1)
                .MakeGenericMethod(targetType);

            var parameter = Expression.Parameter(typeof(MidjourneyVersionsMaster), "v");
            var propertyAccess = Expression.Property(parameter, navigation);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var hasManyBuilder = hasManyMethod.Invoke(builder, new object[] { lambda });

            var withOneMethod = hasManyBuilder.GetType().GetMethod("WithOne", new[] { typeof(string) });
            var hasForeignKeyMethod = hasManyBuilder.GetType().GetMethod("HasForeignKey", new[] { typeof(string[]) });
            var onDeleteMethod = hasManyBuilder.GetType().GetMethod("OnDelete");

            withOneMethod?.Invoke(hasManyBuilder, new object[] { "VersionMaster" });
            hasForeignKeyMethod?.Invoke(hasManyBuilder, new object[] { new[] { "Version" } });
            onDeleteMethod?.Invoke(hasManyBuilder, new object[] { DeleteBehavior.Restrict });
        }
    }
}
