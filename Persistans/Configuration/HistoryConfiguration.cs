using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistans.Constants.Constants;
using Domain.Entities.MidjourneyVersions;
using Domain.Entities.MidjourneyPromtHistory;
using Domain.Entities.MidjourneyStyles;

namespace Persistans.Configuration;

public class PromptHistoryConfiguration : IEntityTypeConfiguration<MidjourneyPromptHistory>
{
    public void Configure(EntityTypeBuilder<MidjourneyPromptHistory> builder)
    {
        builder.ToTable("midjourney_prompt_history", schema: "public");
        builder.HasKey(history => history.HistoryId);

        builder
            .Property(history => history.HistoryId)
            .HasColumnName("history_id")
            .HasColumnType(ColumnType.UniqueIdentifier)
            .ValueGeneratedOnAdd();

        builder
            .Property(history => history.Prompt)
            .HasColumnName("prompt")
            .HasColumnType("text")
            .HasColumnType(ColumnType.NVarChar(1000))
            .IsRequired();

        builder
            .Property(history => history.Version)
            .HasColumnName("properties")
            .HasColumnType(ColumnType.VarChar(7))
            .IsRequired();

        builder
            .Property(history => history.CreatedOn)
            .HasColumnName("created_on")
            .HasColumnType(ColumnType.DateTimeOffset(7))
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder
            .HasOne(history => history.VersionMaster)
            .WithMany(vm => vm.PromptHistories)
            .HasForeignKey("version_master_id")
            .IsRequired();

        builder
            .HasOne(sd => sd.StyleData)
            .WithMany(mph => mph.HistoryID)
            .HasForeignKey(sd => sd.HistoryID);
    }
}
