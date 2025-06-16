//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Domain.Entities.MidjourneyVersions;
//using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
//using static Persistans.Constants.Constants;

//namespace Persistans.Configuration;

//public class MidjourneyVersionConfiguration : IEntityTypeConfiguration<MidjourneyVersion>
//{
//    public void Configure(EntityTypeBuilder<MidjourneyVersion> builder)
//    {
//        builder.HasKey(version => version.Version);

//        builder.Property(version => version.Version)
//            .IsRequired()
//            .HasMaxLength(7)
//            .HasColumnName("version");

//        builder.Property(version => version.VersionAbbrev)
//            .IsRequired()
//            .HasMaxLength(10)
//            .HasColumnName("version_abbrev")
//            .IsRequired();

//        builder.Property(version => version.AspectRatioHeightWidth)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("aspect_ratio/height/width");

//        builder.Property(version => version.Style)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("style");

//        builder.Property(version => version.Stylize)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("stylize");

//        builder.Property(version => version.QualityHdHq)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("quality/hd/hq");

//        builder.Property(version => version.RelaxFastTurbo)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("relax/fast/turbo");

//        builder.Property(version => version.Chaos)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("chaos");

//        builder.Property(version => version.Weird)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("weird");

//        builder.Property(   version => version.Raw)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("raw");

//        builder.Property(version => version.Tile)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("tile");

//        builder.Property(version => version.No)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("no");

//        builder.Property(version => version.Vibe)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("vibe");

//        builder.Property(version => version.TestTestp)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("test/testp");

//        builder.Property(version => version.Creative)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("creative");

//        builder.Property(version => version.Newclip)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("newclip");

//        builder.Property(version => version.Nostretch)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("nostretch");

//        builder.Property(version => version.PromptWeight)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("prompt_weight");

//        builder.Property(version => version.Permutation)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("permutation");

//        builder.Property(version => version.Repeat)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("repeat");

//        builder.Property(version => version.Seed)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("seed");

//        builder.Property(version => version.Sameseed)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("sameseed");

//        builder.Property(version => version.Personalization)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("personalization");

//        builder.Property(version => version.Cref)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("cref");

//        builder.Property(version => version.Sref)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("sref");

//        builder.Property(version => version.Oref)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("oref");

//        builder.Property(version => version.ImagePrompt)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("image_prompt");

//        builder.Property(version => version.ImageWeight)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("image_weight");

//        builder.Property(version => version.Stop)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("stop");

//        builder.Property(version => version.Video)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("video");

//        builder.Property(version => version.Variations)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("variations");

//        builder.Property(version => version.Remix)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("remix");

//        builder.Property(version => version.Editor)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("editor");

//        builder.Property(version => version.FullEditor)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("full_editor");

//        builder.Property(version => version.Upscalers)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("upscalers");

//        builder.Property(version => version.Pan)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("pan");

//        builder.Property(version => version.ZoomOut)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("zoom_out");

//        builder.Property(version => version.Uplight)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("uplight");

//        builder.Property(version => version.Upbeta)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("upbeta");

//        builder.Property(version => version.Upanime)
//            .HasColumnType(ColumnType.Jsonb)
//            .HasColumnName("upanime");
//    }
//}
