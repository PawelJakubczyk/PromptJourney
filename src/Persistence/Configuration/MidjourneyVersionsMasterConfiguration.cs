using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.ValueObjects;
using static Persistence.Mapping.ValueObjectsMapping;
using static Persistence.Constants.PersistenceConstants;
using Domain.Entities;


namespace Persistence.Configuration;

public class MidjourneyVersionsMasterConfiguration : IEntityTypeConfiguration<MidjourneyVersion>
{
    public void Configure(EntityTypeBuilder<MidjourneyVersion> builder)
    {
        builder.ToTable("version_master", schema: "public");
        builder.HasKey(master => master.Version);

        builder.Property(master => master.Version)
            .HasConversion<ModelVersionConverter, ModelVersionComparer>()
            .HasColumnName("version")
            .HasColumnType(ColumnType.VarChar(ModelVersion.MaxLength))
            .IsRequired();

        builder.Property(master => master.Parameter)
            .HasConversion<ParamConverter, ParamComparer>()
            .HasColumnName("parameter")
            .HasColumnType(ColumnType.VarChar(Param.MaxLength))
            .IsRequired();

        builder
            .Property(master => master.ReleaseDate)
            .HasColumnName("release_date")
            .HasColumnType(ColumnType.TimestampWithTimeZone());

        builder
            .Property(master => master.Description)
            .HasConversion<DescriptionConverter, DescriptionComparer>()
            .HasColumnName("description")
            .HasColumnType(ColumnType.Text);

        builder
            .HasMany(master => master.Versions1)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Versions2)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Versions3)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Versions4)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Versions5)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Versions51)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Versions52)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Versions6)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Versions61)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Versions7)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.VersionsNiji4)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.VersionsNiji5)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.VersionsNiji6)
            .WithOne(version => version.VersionMaster)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Histories)
            .WithOne(history => history.VersionMaster)
            .HasForeignKey(history => history.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
