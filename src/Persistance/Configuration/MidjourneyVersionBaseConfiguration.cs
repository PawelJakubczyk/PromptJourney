using Domain.Entities.MidjourneyVersions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistans.Constants.Constants;

namespace Persistans.Configuration;

public abstract class MidjourneyVersionBaseConfiguration<T> : IEntityTypeConfiguration<T>
    where T : MidjourneyVersionsBase
{
    internal static string? TableName { get; set; }

    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(TableName!, schema: "public");

        builder.HasKey(versin => versin.PropertyName);

        builder
            .Property(version => version.PropertyName)
            .HasColumnName("property_name")
            .HasColumnType(ColumnType.VarChar(25))
            .IsRequired();

        builder.Property(version => version.Version)
            .HasColumnName("version")
            .HasColumnType(ColumnType.VarChar(10))
            .IsRequired();

        builder.Property(version => version.Parameters)
            .HasColumnName("parameters")
            .HasColumnType(ColumnType.textArray)
            .IsRequired();

        builder.Property(version => version.DefaultValue)
            .HasColumnName("default_value")
            .HasColumnType(ColumnType.VarChar(50));

        builder.Property(version => version.MinValue)
            .HasColumnName("min_value")
            .HasColumnType(ColumnType.VarChar(50));

        builder.Property(version => version.MaxValue)
            .HasColumnName("max_value")
            .HasColumnType(ColumnType.VarChar(50));

        builder.Property(version => version.Description)
            .HasColumnName("description")
            .HasColumnType(ColumnType.text);
    }
}

public class Version1Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersion1> 
{
    static Version1Configuration()
    {
        TableName = "version_1";
    }
}
public class Version2Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersion2>
{
    static Version2Configuration()
    {
        TableName = "version_2";
    }
}

public class Version3Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersion3>
{
    static Version3Configuration()
    {
        TableName = "version_3";
    }
}

public class Version4Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersion4>
{
    static Version4Configuration()
    {
        TableName = "version_4";
    }
}

public class Version5Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersion5>
{
    static Version5Configuration()
    {
        TableName = "version_5";
    }
}

public class Version51Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersion51>
{
    static Version51Configuration()
    {
        TableName = "version_5_1";
    }
}

public class Version52Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersion52>
{
    static Version52Configuration()
    {
        TableName = "version_5_2";
    }
}

public class Version6Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersion6>
{
    static Version6Configuration()
    {
        TableName = "version_6";
    }
}

public class Version61Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersion61>
{
    static Version61Configuration()
    {
        TableName = "version_6_1";
    }
}

public class Version7Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersion7>
{
    static Version7Configuration()
    {
        TableName = "version_7";
    }
}

public class VersionNiji4Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersionNiji4>
{
    static VersionNiji4Configuration()
    {
        TableName = "version_niji_4";
    }
}

public class VersionNiji5Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersionNiji5>
{
    static VersionNiji5Configuration()
    {
        TableName = "version_niji_5";
    }
}

public class VersionNiji6Configuration : MidjourneyVersionBaseConfiguration<MidjourneyAllVersions.MidjourneyVersionNiji6>
{
    static VersionNiji6Configuration()
    {
        TableName = "version_niji_6";
    }
}


