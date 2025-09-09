using Domain.Entities.MidjourneyStyleExampleLinks;
using Domain.Entities.MidjourneyStyle;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistence.Constants.PersistenceConstants;
using static Persistence.ConventersComparers.ValueObjects.ModelVersionMapping;
using static Persistence.Mapping.ValueObjects.ExampleLinkMapping;
using static Persistence.Mapping.ValueObjects.StyleNameMapping;

namespace Persistence.Configuration;

public class MidjourneyStyleExampleLinkConfiguration : IEntityTypeConfiguration<MidjourneyStyleExampleLink>
{
    public void Configure(EntityTypeBuilder<MidjourneyStyleExampleLink> builder)
    {
        builder.ToTable("midjourney_style_example_links", schema: "public");
        
        // Composite primary key
        builder.HasKey(el => new { el.Link, el.StyleName, el.Version });
        
        builder.Property(el => el.Link)
            .HasConversion<ExampleLinkConverter, ExampleLinkComparer>()
            .HasColumnName("link")
            .HasColumnType(ColumnType.VarChar(ExampleLink.MaxLength))
            .IsRequired();
            
        builder.Property(el => el.StyleName)
            .HasConversion<StyleNameConverter, StyleNameComparer>()
            .HasColumnName("style_name")
            .HasColumnType(ColumnType.VarChar(StyleName.MaxLength))
            .IsRequired();
            
        builder.Property(el => el.Version)
            .HasConversion<ModelVersionConverter, ModelVersionComparer>()
            .HasColumnName("version")
            .HasColumnType(ColumnType.VarChar(ModelVersion.MaxLength))
            .IsRequired();
            
        // Configure relationships
        builder.HasOne(el => el.Style)
            .WithMany(s => s.ExampleLinks)
            .HasForeignKey(el => el.StyleName)
            .HasPrincipalKey(s => s.StyleName)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(el => el.VersionMaster)
            .WithMany()
            .HasForeignKey(el => el.Version)
            .HasPrincipalKey(vm => vm.Version)
            .OnDelete(DeleteBehavior.Cascade);
    }
}