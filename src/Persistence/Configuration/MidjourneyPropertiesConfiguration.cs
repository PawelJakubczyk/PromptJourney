using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistence.Mapping.ValueObjectsMapping;
using static Persistence.Constants.PersistenceConstants;
using Domain.Entities;

namespace Persistence.Configuration;

public abstract class MidjourneyPropertiesConfiguration<T> : IEntityTypeConfiguration<T>
    where T : MidjourneyProperties
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable("properties", schema: "public");

        builder.HasKey(version => version.PropertyName);

        builder
            .Property(version => version.PropertyName)
            .HasConversion<PropertyNameConverter, PropertyNameComparer>()
            .HasColumnName("property_name")
            .HasColumnType(ColumnType.VarChar(PropertyName.MaxLength))
            .IsRequired();

        builder.Property(version => version.Version)
            .HasConversion<ModelVersionConverter, ModelVersionComparer>()
            .HasColumnName("version")
            .HasColumnType(ColumnType.VarChar(ModelVersion.MaxLength))
            .IsRequired();

        builder.Property(version => version.Parameters)
            .HasConversion<ParamListConverter, ParamListComparer>()
            .HasColumnName("parameters")
            .HasColumnType(ColumnType.TextArray)
            .IsRequired();

        builder.Property(version => version.DefaultValue)
            .HasConversion<DefaultValueConverter, DefaultValueComparer>()
            .HasColumnName("default_value")
            .HasColumnType(ColumnType.VarChar(DefaultValue.MaxLength));

        builder.Property(version => version.MinValue)
            .HasConversion<MinValueConverter, MinValueComparer>()
            .HasColumnName("min_value")
            .HasColumnType(ColumnType.VarChar(MinValue.MaxLength));

        builder.Property(version => version.MaxValue)
            .HasConversion<MaxValueConverter, MaxValueComparer>()
            .HasColumnName("max_value")
            .HasColumnType(ColumnType.VarChar(MaxValue.MaxLength));

        builder.Property(version => version.Description)
            .HasConversion<DescriptionConverter, DescriptionComparer>()
            .HasColumnName("description")
            .HasColumnType(ColumnType.Text);

        // Foreign key relationship
        builder
            .HasOne(version => version.VersionMaster)
            .WithMany()
            .HasForeignKey(version => version.Version)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


