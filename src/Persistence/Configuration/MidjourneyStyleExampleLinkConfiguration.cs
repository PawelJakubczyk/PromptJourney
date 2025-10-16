using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistence.Constants.PersistenceConstants;
using static Persistence.Mapping.ValueObjectsMapping;

namespace Persistence.Configuration;

public class MidjourneyStyleExampleLinkConfiguration : IEntityTypeConfiguration<MidjourneyStyleExampleLink>
{
    public void Configure(EntityTypeBuilder<MidjourneyStyleExampleLink> builder)
    {
        builder.ToTable("midjourney_style_example_links", schema: "public");

        // Primary key - now using Guid Id
        builder.HasKey(link => link.Id);

        builder.Property(link => link.Id)
            .HasColumnName("id")
            .HasColumnType(ColumnType.Uuid)
            .IsRequired();

        builder.Property(link => link.Link)
            .HasConversion<LinkConverter, LinkComparer>()
            .HasColumnName("link")
            .HasColumnType(ColumnType.VarChar(ExampleLink.MaxLength))
            .IsRequired();

        builder.Property(link => link.StyleName)
            .HasConversion<StyleNameConverter, StyleNameComparer>()
            .HasColumnName("style_name")
            .HasColumnType(ColumnType.VarChar(StyleName.MaxLength))
            .IsRequired();

        builder.Property(link => link.Version)
            .HasConversion<ModelVersionConverter, ModelVersionComparer>()
            .HasColumnName("version")
            .HasColumnType(ColumnType.VarChar(ModelVersion.MaxLength))
            .IsRequired();

        // Configure relationships
        builder.HasOne(link => link.MidjuorneyStyle)
            .WithMany(style => style.MidjourneyExampleLinks)
            .HasForeignKey(link => link.StyleName)
            .HasPrincipalKey(style => style.StyleName)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(link => link.MidjourneyMaster)
            .WithMany()
            .HasForeignKey(link => link.Version)
            .HasPrincipalKey(version => version.Version)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder
            .HasIndex(link => link.StyleName)
            .HasDatabaseName("IX_midjourney_style_example_links_style_name");

        builder
            .HasIndex(link => link.Version)
            .HasDatabaseName("IX_midjourney_style_example_links_version");
    }
}
