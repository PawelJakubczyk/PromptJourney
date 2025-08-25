using Domain.Entities.MidjourneyPromtHistory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
            .HasColumnType(ColumnType.uuid);

        builder
            .Property(history => history.Prompt)
            .HasColumnName("prompt")
            .HasColumnType(ColumnType.VarChar(1000))
            .IsRequired();

        builder
            .Property(history => history.Version)
            .HasColumnName("properties")
            .HasColumnType(ColumnType.VarChar(7))
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
