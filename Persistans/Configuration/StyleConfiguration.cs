using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.MidjourneyStyles;
using Microsoft.VisualBasic;
using static Persistans.Constants.Constants;

namespace Persistans.Configuration;

public class StyleConfiguration : IEntityTypeConfiguration<MidjourneyStyle>
{
    public void Configure(EntityTypeBuilder<MidjourneyStyle> builder)
    {
        builder.ToTable("Styles");
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
            .HasMany(style => style.MidjourneyPromptHistory)
            .WithOne(history => history.HistoryID)
            .UsingEntity(j => j.ToTable("PromptHistoryStyles"));
    }
}
