using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.MidjourneyStyles;
using static Persistans.Constants.Constants;

namespace Persistans.Configuration;

public class StyleDataConfiguration : IEntityTypeConfiguration<StyleData>
{
    public void Configure(EntityTypeBuilder<StyleData> builder)
    {
        builder.ToTable("StyleDatas");
        builder.HasKey(style => new { style.HistoryID, style.MidjourneyStyleName });

        builder
            .Property(style => style.MidjourneyStyleName)
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
            .HasMany(style => style.MidjourneyPromptHistory)
            .WithMany(history => history.Styles)
            .UsingEntity(j => j.ToTable("PromptHistoryStyles"));
    }
}
