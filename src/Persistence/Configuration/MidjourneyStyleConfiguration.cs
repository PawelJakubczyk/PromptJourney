using Domain.Entities.MidjourneyStyle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.ValueObjects;
using static Persistence.Mapping.ValueObjects;
using static Persistence.Constants.PersistenceConstants;

namespace Persistence.Configuration;

public class MidjourneyStyleConfiguration : IEntityTypeConfiguration<MidjourneyStyle>
{
    public void Configure(EntityTypeBuilder<MidjourneyStyle> builder)
    {
        builder.ToTable("midjourney_styles", schema: "public");
        builder.HasKey(style => style.StyleName);
        
        builder.Property(style => style.StyleName)
            .HasConversion<StyleNameMapping.Converter, StyleNameMapping.Comparer>()
            .HasColumnName("name")
            .HasColumnType(ColumnType.VarChar(StyleName.MaxLength))
            .IsRequired();
            
        builder.Property(style => style.Type)
            .HasConversion<StyleTypeMapping.Converter, StyleTypeMapping.Comparer>()
            .HasColumnName("type")
            .HasColumnType(ColumnType.VarChar(StyleType.MaxLength))
            .IsRequired();
            
        builder.Property(style => style.Description)
            .HasConversion<DescriptionMapping.Converter, DescriptionMapping.Comparer>()
            .HasColumnName("description")
            .HasColumnType(ColumnType.VarChar(Description.MaxLength));
            
        builder.Property(style => style.Tags)
            .HasConversion<TagListMapping.Converter, TagListMapping.Comparer>()
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
    }
}
