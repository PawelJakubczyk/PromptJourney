using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.ValueObjects;
using static Persistence.Mapping.ValueObjectsMapping;
using static Persistence.Constants.PersistenceConstants;
using Domain.Entities;


namespace Persistence.Configuration;

public class MidjourneyVersionConfiguration : IEntityTypeConfiguration<MidjourneyVersion>
{
    public void Configure(EntityTypeBuilder<MidjourneyVersion> builder)
    {
        builder.ToTable("midjourney_versions", schema: "public");
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
            .HasMany(master => master.Properties)
            .WithOne(version => version.MidjourneyVersion)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Properties)
            .WithOne(version => version.MidjourneyVersion)
            .HasForeignKey(version => version.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(master => master.Histories)
            .WithOne(history => history.MidjourneyVersion)
            .HasForeignKey(history => history.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
