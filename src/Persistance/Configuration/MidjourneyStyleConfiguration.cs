using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.MidjourneyStyles;
using static Persistans.Constants.Constants;

namespace Persistans.Configuration;

public class MidjourneyStyleConfiguration : IEntityTypeConfiguration<MidjourneyStyle>
{
    public void Configure(EntityTypeBuilder<MidjourneyStyle> builder)
    {
        builder.ToTable("midjourney_styles", schema: "public");
        builder.HasKey(style => style.Name);

        builder
            .Property(style => style.Name)
            .HasColumnName("name")
            .HasColumnType(ColumnType.VarChar(150))
            .IsRequired();

        builder
            .Property(style => style.Type)
            .HasColumnName("type")
            .HasColumnType(ColumnType.VarChar(100))
            .IsRequired();

        builder
            .Property(style => style.Description)
            .HasColumnName("description")
            .HasColumnType(ColumnType.VarChar(800));

        builder
            .Property(style => style.Tags)
            .HasColumnName("tags")
            .HasColumnType(ColumnType.textArray);

        builder
            .Property(style => style.ExampleLinks)
            .HasColumnName("example_links")
            .HasColumnType(ColumnType.textArray);
    }
}
