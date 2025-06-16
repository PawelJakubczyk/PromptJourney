using Domain.Entities.MidjourneyVersions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistans.Constants.Constants;

namespace Persistans.Configuration;

public class MidjourneyVersionsMasterConfiguration : IEntityTypeConfiguration<MidjourneyVersionsMaster>
{
    public void Configure(EntityTypeBuilder<MidjourneyVersionsMaster> builder)
    {
        builder.ToTable("version_master", schema: "public");
        builder.HasKey(master => master.Version);

        builder.Property(master => master.Version)
            .HasColumnName("version")
            .HasColumnType(ColumnType.NVarChar(10))
            .IsRequired();

        builder
            .Property(master => master.ReleaseDate)
            .HasColumnName("release_date")
            .HasColumnType(ColumnType.DateTimeOffset(7));

        builder
            .Property(master => master.Description)
            .HasColumnName("description")
            .HasColumnType(ColumnType.Text);

        //builder
        //    .HasMany(master => master.Versions1)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.Versions2)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.Versions3)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.Versions4)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.Versions5)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.Versions51)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.Versions52)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.Versions6)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.Versions61)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.Versions7)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.VersionsNiji4)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.VersionsNiji5)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(master => master.VersionsNiji6)
        //    .WithOne(version => version.VersionMaster)
        //    .HasForeignKey(version => version.PropertyName)
        //    .HasPrincipalKey(master => master.Version)
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}
