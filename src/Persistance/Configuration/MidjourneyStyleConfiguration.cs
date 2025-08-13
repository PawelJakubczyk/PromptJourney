using Domain.Entities.MidjourneyStyles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistance.Constants.PersistansConstants;

namespace Persistance.Configuration;

public class MidjourneyStyleConfiguration : IEntityTypeConfiguration<MidjourneyStyle>
{
    public void Configure(EntityTypeBuilder<MidjourneyStyle> builder)
    {
        builder.ToTable("midjourney_styles", schema: "public");
        builder.HasKey(style => style.Name);
        
        builder.Property(style => style.Name)
            .HasColumnName("name")
            .HasColumnType(ColumnType.VarChar(100))
            .IsRequired();
            
        builder.Property(style => style.Type)
            .HasColumnName("type")
            .HasColumnType(ColumnType.VarChar(50))
            .IsRequired();
            
        builder.Property(style => style.Description)
            .HasColumnName("description")
            .HasColumnType(ColumnType.VarChar(500));
            
        builder.Property(style => style.Tags)
            .HasColumnName("tags")
            .HasColumnType(ColumnType.textArray);
    }
}
