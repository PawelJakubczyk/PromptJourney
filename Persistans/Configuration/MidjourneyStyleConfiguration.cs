using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.MidjourneyStyles;
using static Persistans.Constants.Constants;

namespace Persistans.Configuration;

public class MidjourneyStyleConfiguration : IEntityTypeConfiguration<MidjourneyStyle>
{
    public void Configure(EntityTypeBuilder<MidjourneyStyle> builder)
    {
        builder.ToTable("MidjourneyStyles", schema: "public");
        builder.HasKey(style => style.Name);

        builder
            .Property(style => style.Name)
            .HasColumnName("name")
            .HasColumnType(ColumnType.NVarChar(150))
            .IsRequired();

        builder
            .Property(style => style.Type)
            .HasColumnName("type")
            .HasColumnType(ColumnType.NVarChar(100))
            .IsRequired();

        builder
            .Property(style => style.Description)
            .HasColumnName("description")
            .HasColumnType(ColumnType.NVarChar(800))
            .IsRequired(false);

        builder
            .Property(style => style.Tags)        
            .HasColumnName("tags") 
            .HasColumnType(ColumnType.TextArray)
            .IsRequired(false);

        builder
            .Property(style => style.ExampleLinks)
            .HasColumnName("example_links")
            .HasColumnType(ColumnType.TextArray)
            .IsRequired(false);

        builder
            .HasMany(style => style.MidjourneyPromptHistories)
            .WithMany(history => history.MidjourneyStyles);
    }
}
