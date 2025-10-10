using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistence.Constants.PersistenceConstants;
using static Persistence.Mapping.ValueObjectsMapping;

namespace Persistence.Configuration;

public class MidjourneyPropertiesConfiguration : IEntityTypeConfiguration<MidjourneyProperties>
{
    public virtual void Configure(EntityTypeBuilder<MidjourneyProperties> builder)
    {
        builder.ToTable("midjourney_properties", schema: "public");

        // COMPOSITE KEY - PropertyName + Version
        builder.HasKey(p => new { p.PropertyName, p.Version });

        // PropertyName configuration
        builder
            .Property(property => property.PropertyName)
            .HasConversion<PropertyNameConverter, PropertyNameComparer>()
            .HasColumnName("property_name")
            .HasColumnType(ColumnType.VarChar(PropertyName.MaxLength))
            .IsRequired();

        builder.Property(property => property.Version)
            .HasConversion<ModelVersionConverter, ModelVersionComparer>()
            .HasColumnName("version")
            .HasColumnType(ColumnType.VarChar(ModelVersion.MaxLength))
            .IsRequired();

        builder.Property(property => property.Parameters)
            .HasConversion<ParamListConverter, ParamListComparer>()
            .HasColumnName("parameters")
            .HasColumnType(ColumnType.TextArray)
            .IsRequired();

        builder.Property(property => property.DefaultValue)
            .HasConversion<DefaultValueConverter, DefaultValueComparer>()
            .HasColumnName("default_value")
            .HasColumnType(ColumnType.VarChar(DefaultValue.MaxLength));

        builder.Property(property => property.MinValue)
            .HasConversion<MinValueConverter, MinValueComparer>()
            .HasColumnName("min_value")
            .HasColumnType(ColumnType.VarChar(MinValue.MaxLength));

        builder.Property(property => property.MaxValue)
            .HasConversion<MaxValueConverter, MaxValueComparer>()
            .HasColumnName("max_value")
            .HasColumnType(ColumnType.VarChar(MaxValue.MaxLength));

        builder.Property(property => property.Description)
            .HasConversion<DescriptionConverter, DescriptionComparer>()
            .HasColumnName("description")
            .HasColumnType(ColumnType.Text);

        // Foreign key relationship
        builder
            .HasOne(version => version.MidjourneyVersion)
            .WithMany()
            .HasForeignKey(version => version.Version)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder
            .HasIndex(p => p.Version)
            .HasDatabaseName("IX_midjourney_properties_version");
        builder
            .HasIndex(p => p.PropertyName)
            .HasDatabaseName("IX_midjourney_properties_property_name");
    }
}