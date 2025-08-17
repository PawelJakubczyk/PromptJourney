using Domain.Entities.MidjourneyVersions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistance.Constants.PersistansConstants;

namespace Persistance.Configuration;

public class MidjourneyVersionsMasterConfiguration : IEntityTypeConfiguration<MidjourneyVersions>
{
    public void Configure(EntityTypeBuilder<MidjourneyVersions> builder)
    {
        builder.ToTable("version_master", schema: "public");
        builder.HasKey(master => master.Version);

        builder.Property(master => master.Version)
            .HasColumnName("version")
            .HasColumnType(ColumnType.VarChar(10))
            .IsRequired();

        builder.Property(master => master.Parameter)
            .HasColumnName("parameter")
            .HasColumnType(ColumnType.VarChar(15))
            .IsRequired();

        builder
            .Property(master => master.ReleaseDate)
            .HasColumnName("release_date")
            .HasColumnType(ColumnType.TimestampWithTimeZone());

        builder
            .Property(master => master.Description)
            .HasColumnName("description")
            .HasColumnType(ColumnType.text);

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
    }
}
