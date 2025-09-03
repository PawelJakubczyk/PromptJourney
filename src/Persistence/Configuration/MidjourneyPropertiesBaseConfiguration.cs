using Domain.Entities.MidjourneyProperties;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistence.Constants.PersistenceConstants;
using static Persistence.ConventersComparers.ValueObjects.ModelVersionMapping;
using static Persistence.Mapping.ValueObjects.PropertyNameMapping;
using static Persistence.Mapping.ValueObjects.ParamMapping;
using static Persistence.Mapping.ValueObjects.DefaultValueMapping;
using static Persistence.Mapping.ValueObjects.MinValueMapping;
using static Persistence.Mapping.ValueObjects.MaxValueMapping;
using static Persistence.Mapping.ValueObjects.DescriptionMapping;

namespace Persistence.Configuration;

public abstract class MidjourneyPropertiesBaseConfiguration<T> : IEntityTypeConfiguration<T>
    where T : MidjourneyPropertiesBase
{
    internal static string? TableName { get; set; }

    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(TableName!, schema: "public");

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
            .WithMany() // Set appropriate navigation property based on version
            .HasForeignKey(version => version.Version)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// All version-specific configurations remain the same
public class PropertiesVersion1Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion1> 
{
    static PropertiesVersion1Configuration()
    {
        TableName = "properties_version_1";
    }
}

public class PropertiesVersion2Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion2>
{
    static PropertiesVersion2Configuration()
    {
        TableName = "properties_version_2";
    }
}

public class PropertiesVersion3Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion3>
{
    static PropertiesVersion3Configuration()
    {
        TableName = "properties_version_3";
    }
}

public class PropertiesVersion4Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion4>
{
    static PropertiesVersion4Configuration()
    {
        TableName = "properties_version_4";
    }
}

public class PropertiesVersion5Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion5>
{
    static PropertiesVersion5Configuration()
    {
        TableName = "properties_version_5";
    }
}

public class PropertiesVersion51Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion51>
{
    static PropertiesVersion51Configuration()
    {
        TableName = "properties_version_5_1";
    }
}

public class PropertiesVersion52Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion52>
{
    static PropertiesVersion52Configuration()
    {
        TableName = "properties_version_5_2";
    }
}

public class PropertiesVersion6Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion6>
{
    static PropertiesVersion6Configuration()
    {
        TableName = "properties_version_6";
    }
}

public class PropertiesVersion61Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion61>
{
    static PropertiesVersion61Configuration()
    {
        TableName = "properties_version_6_1";
    }
}

public class PropertiesVersion7Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion7>
{
    static PropertiesVersion7Configuration()
    {
        TableName = "properties_version_7";
    }
}

public class PropertiesVersionNiji4Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji4>
{
    static PropertiesVersionNiji4Configuration()
    {
        TableName = "properties_version_niji_4";
    }
}

public class PropertiesVersionNiji5Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji5>
{
    static PropertiesVersionNiji5Configuration()
    {
        TableName = "properties_version_niji_5";
    }
}

public class PropertiesVersionNiji6Configuration 
    : MidjourneyPropertiesBaseConfiguration<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji6>
{
    static PropertiesVersionNiji6Configuration()
    {
        TableName = "properties_version_niji_6";
    }
}


