using Domain.Entities.MidjourneyStyle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.ValueObjects;
using StyleNameConverter = Persistence.Mapping.ValueObjects.StyleNameMapping.Converter;
using StyleNameComparer = Persistence.Mapping.ValueObjects.StyleNameMapping.Comparer;
using StyleTypeConverter = Persistence.Mapping.ValueObjects.StyleTypeMapping.Converter;
using StyleTypeComparer = Persistence.Mapping.ValueObjects.StyleTypeMapping.Comparer;
using DescriptionConverter = Persistence.Mapping.ValueObjects.DescriptionMapping.Converter;
using DescriptionComparer = Persistence.Mapping.ValueObjects.DescriptionMapping.Comparer;
using TagListConverter = Persistence.Mapping.ValueObjects.TagListMapping.Converter;
using TagListComparer = Persistence.Mapping.ValueObjects.TagListMapping.Comparer;
using static Persistence.Constants.PersistenceConstants;

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
    }
}
