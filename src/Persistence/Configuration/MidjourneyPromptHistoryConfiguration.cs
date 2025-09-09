using Domain.Entities.MidjourneyPromtHistory;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromptConverter = Persistence.Mapping.ValueObjects.PromptMapping.Converter;
using PromptComparer =  Persistence.Mapping.ValueObjects.PromptMapping.Comparer;
using ModelVersionConverter = Persistence.Mapping.ValueObjects.ModelVersionMapping.Converter;
using ModelVersionComparer =  Persistence.Mapping.ValueObjects.ModelVersionMapping.Comparer;
using static Persistence.Constants.PersistenceConstants;

namespace Persistence.Configuration;

public class MidjourneyPromptHistoryConfiguration : IEntityTypeConfiguration<MidjourneyPromptHistory>
{
    public void Configure(EntityTypeBuilder<MidjourneyPromptHistory> builder)
    {
        builder.ToTable("midjourney_prompt_history", schema: "public");
        builder.HasKey(history => history.HistoryId);

        builder
            .Property(history => history.HistoryId)
            .HasColumnName("history_id")
            .HasColumnType(ColumnType.Uuid);

        builder
            .Property(history => history.Prompt)
            .HasConversion<PromptConverter, PromptComparer>()
            .HasColumnName("prompt")
            .HasColumnType(ColumnType.VarChar(Prompt.MaxLength))
            .IsRequired();

        builder
            .Property(history => history.Version)
            .HasConversion<ModelVersionConverter, ModelVersionComparer>()
            .HasColumnName("properties")
            .HasColumnType(ColumnType.VarChar(ModelVersion.MaxLength))
            .IsRequired();

        builder
            .Property(history => history.CreatedOn)
            .HasColumnName("created_on")
            .HasColumnType(ColumnType.TimestampWithTimeZone())
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder
            .HasOne(history => history.VersionMaster)
            .WithMany(master => master.Histories)
            .HasForeignKey(history => history.Version)
            .HasPrincipalKey(master => master.Version)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(history => history.MidjourneyStyles)
            .WithMany(style => style.MidjourneyPromptHistories);
    }
}
