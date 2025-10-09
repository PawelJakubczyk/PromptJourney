using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistence.Constants.PersistenceConstants;
using static Persistence.Mapping.ValueObjectsMapping;

namespace Persistence.Configuration;

public class MidjourneyStyleConfiguration : IEntityTypeConfiguration<MidjourneyStyle>
{
    public void Configure(EntityTypeBuilder<MidjourneyStyle> builder)
    {
        builder.ToTable("midjourney_styles", schema: "public");
        builder.HasKey(style => style.StyleName);

        builder.Property(style => style.StyleName)
            .HasConversion<StyleNameConverter, StyleNameComparer>()
            .HasColumnName("name")
            .HasColumnType(ColumnType.VarChar(StyleName.MaxLength))
            .IsRequired();

        builder.Property(style => style.Type)
            .HasConversion<StyleTypeConverter, StyleTypeComparer>()
            .HasColumnName("type")
            .HasColumnType(ColumnType.VarChar(StyleType.MaxLength))
            .IsRequired();

        builder.Property(style => style.Description)
            .HasConversion<DescriptionConverter, DescriptionComparer>()
            .HasColumnName("description")
            .HasColumnType(ColumnType.VarChar(Description.MaxLength));

        builder.Property(style => style.Tags)
            .HasConversion<TagListConverter, TagListComparer>()
            .HasColumnName("tags")
            .HasColumnType(ColumnType.TextArray);

        // Navigation properties
        builder
            .HasMany(style => style.ExampleLinks)
            .WithOne(link => link.Style)
            .HasForeignKey(link => link.StyleName)
            .HasPrincipalKey(style => style.StyleName)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(style => style.MidjourneyPromptHistories)
            .WithMany(history => history.MidjourneyStyles);

        // Indexes for performance
        builder
            .HasIndex(p => p.Type)
            .HasDatabaseName("IX_midjourney_styles_type");
        builder
            .HasIndex(p => p.Tags)
            .HasDatabaseName("IX_midjourney_styles_tags");
    }
}