using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.MidjourneyVersions;

namespace Persistans.Configuration;

public abstract class BaseVersionConfiguration<T> : IEntityTypeConfiguration<T>
    where T : MidjourneyVersionsBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(versin => versin.PropertyName);

        builder.Property(version => version.Version)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasOne(version => version.VersionMaster)
            .WithMany()
            .HasForeignKey(versin => versin.Version)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(version => version.Parameter)
            .HasMaxLength(50);

        builder.Property(version => version.DefaultValue)
            .HasMaxLength(50);

        builder.Property(version => version .MinValue)
            .HasMaxLength(50);

        builder.Property(version => version.MaxValue)
            .HasMaxLength(50);

        builder.Property(version => version.Description)
            .HasColumnType("text");

        builder.Property(version => version.Modes)
            .HasColumnType("text[]");

        builder.Property(version => version.ParameterModes)
            .HasColumnType("text[]");
    }
}

public class Version1Configuration : BaseVersionConfiguration<MidjourneyVersion1> { }
public class Version2Configuration : BaseVersionConfiguration<MidjourneyVersion2> { }
public class Version3Configuration : BaseVersionConfiguration<MidjourneyVersion3> { }
public class Version4Configuration : BaseVersionConfiguration<MidjourneyVersion4> { }
public class Version5Configuration : BaseVersionConfiguration<MidjourneyVersion5> { }
public class Version51Configuration : BaseVersionConfiguration<MidjourneyVersion51> { }
public class Version52Configuration : BaseVersionConfiguration<MidjourneyVersion52> { }
public class Version6Configuration : BaseVersionConfiguration<MidjourneyVersion6> { }
public class Version61Configuration : BaseVersionConfiguration<MidjourneyVersion61> { }
public class Version7Configuration : BaseVersionConfiguration<MidjourneyVersion7> { }
public class VersionNiji4Configuration : BaseVersionConfiguration<MidjourneyVersionNiji4> { }
public class VersionNiji5Configuration : BaseVersionConfiguration<MidjourneyVersionNiji5> { }
public class VersionNiji6Configuration : BaseVersionConfiguration<MidjourneyVersionNiji6> { }

