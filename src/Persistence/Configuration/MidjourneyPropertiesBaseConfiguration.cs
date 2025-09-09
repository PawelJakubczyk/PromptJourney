using Domain.Entities.MidjourneyProperties;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyNameConverter = Persistence.Mapping.ValueObjects.PropertyNameMapping.Converter;
using PropertyNameComparer = Persistence.Mapping.ValueObjects.PropertyNameMapping.Comparer;
using ParamListConverter = Persistence.Mapping.ValueObjects.ParamListMapping.Converter;
using ParamListComparer = Persistence.Mapping.ValueObjects.ParamListMapping.Comparer;
using ModelVersionConverter = Persistence.Mapping.ValueObjects.ModelVersionMapping.Converter;
using ModelVersionComparer = Persistence.Mapping.ValueObjects.ModelVersionMapping.Comparer;
using DefaultValueConverter = Persistence.Mapping.ValueObjects.DefaultValueMapping.Converter;
using DefaultValueComparer = Persistence.Mapping.ValueObjects.DefaultValueMapping.Comparer;
using MinValueConverter = Persistence.Mapping.ValueObjects.MinValueMapping.Converter;
using MinValueComparer = Persistence.Mapping.ValueObjects.MinValueMapping.Comparer;
using MaxValueConverter = Persistence.Mapping.ValueObjects.MaxValueMapping.Converter;
using MaxValueComparer = Persistence.Mapping.ValueObjects.MaxValueMapping.Comparer;
using DescriptionConverter = Persistence.Mapping.ValueObjects.DescriptionMapping.Converter;
using DescriptionComparer = Persistence.Mapping.ValueObjects.DescriptionMapping.Comparer;
using static Persistence.Constants.PersistenceConstants;

namespace Persistence.Configuration;

public abstract class MidjourneyPropertiesBaseConfiguration<T> : IEntityTypeConfiguration<T>
    where T : MidjourneyPropertiesBase
{
    protected abstract string TableName { get; }

    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(TableName, schema: "public");

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

public class PropertiesVersion1Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion1> 
{
    protected override string TableName => "properties_version_1";
}

public class PropertiesVersion2Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion2>
{
    protected override string TableName => "properties_version_2";
}

public class PropertiesVersion3Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion3>
{
    protected override string TableName => "properties_version_3";
}

public class PropertiesVersion4Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion4>
{
    protected override string TableName => "properties_version_4";
}

public class PropertiesVersion5Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion5>
{
    protected override string TableName => "properties_version_5";
}

public class PropertiesVersion51Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion51>
{
    protected override string TableName => "properties_version_5_1";
}

public class PropertiesVersion52Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion52>
{
    protected override string TableName => "properties_version_5_2";
}

public class PropertiesVersion6Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion6>
{
    protected override string TableName => "properties_version_6";
}

public class PropertiesVersion61Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion61>
{
    protected override string TableName => "properties_version_6_1";
}

public class PropertiesVersion7Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion7>
{
    protected override string TableName => "properties_version_7";
}

public class PropertiesVersionNiji4Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji4>
{
    protected override string TableName => "properties_version_niji_4";
}

public class PropertiesVersionNiji5Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji5>
{
    protected override string TableName => "properties_version_niji_5";
}

public class PropertiesVersionNiji6Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji6>
{
    protected override string TableName => "properties_version_niji_6";
}


