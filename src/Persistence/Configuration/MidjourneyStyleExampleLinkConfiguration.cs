using Domain.Entities.MidjourneyStyles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistence.Constants.PersistenceConstants;

namespace Persistence.Configuration;

public class MidjourneyStyleExampleLinkConfiguration : IEntityTypeConfiguration<MidjourneyStyleExampleLink>
{
    public void Configure(EntityTypeBuilder<MidjourneyStyleExampleLink> builder)
    {
        builder.ToTable("midjourney_style_example_links", schema: "public");
        
        // Composite primary key
        builder.HasKey(el => new { el.Link, el.StyleName, el.Version });
        
        builder.Property(el => el.Link)
            .HasColumnName("link")
            .HasColumnType(ColumnType.VarChar(200))
            .IsRequired();
            
        builder.Property(el => el.StyleName)
            .HasColumnName("style_name")
            .HasColumnType(ColumnType.VarChar(100))
            .IsRequired();
            
        builder.Property(el => el.Version)
            .HasColumnName("version")
            .HasColumnType(ColumnType.VarChar(10))
            .IsRequired();
            
        // Configure relationships
        builder.HasOne(el => el.Style)
            .WithMany(s => s.ExampleLinks)
            .HasForeignKey(el => el.StyleName)
            .HasPrincipalKey(s => s.Name)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(el => el.VersionMaster)
            .WithMany()
            .HasForeignKey(el => el.Version)
            .HasPrincipalKey(vm => vm.Version)
            .OnDelete(DeleteBehavior.Cascade);
    }
}