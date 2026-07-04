using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Persistence.Constants.PersistenceConstants;
using static Persistence.Mapping.ValueObjectsMapping;

namespace Persistence.Configuration;

public class MidjourneyUserConfiguration : IEntityTypeConfiguration<MidjourneyUser>
{
    public void Configure(EntityTypeBuilder<MidjourneyUser> builder)
    {
        builder.ToTable("midjourney_user", schema: "public");
        builder.HasKey(user => user.UserId);

        builder
            .Property(user => user.UserId)
            .HasConversion<UserIDConverter, UserIDComparer>()
            .HasColumnName("user_id")
            .HasColumnType(ColumnType.Uuid)
            .IsRequired();

        builder
            .Property(user => user.UserName)
            .HasConversion<UserNameConverter, UserNameComparer>()
            .HasColumnName("user_name")
            .HasColumnType(ColumnType.VarChar(UserName.MaxLength))
            .IsRequired();

        builder
            .Property(user => user.Email)
            .HasConversion<EmailConverter, EmailComparer>()
            .HasColumnName("email")
            .HasColumnType(ColumnType.VarChar(Email.MaxLength))
            .IsRequired();

        builder
            .Property(user => user.PasswordHash)
            .HasConversion<PasswordHashConverter, PasswordHashComparer>()
            .HasColumnName("password_hash")
            .HasColumnType(ColumnType.VarChar(PasswordHash.MaxLength))
            .IsRequired();

        builder
            .Property(user => user.Role)
            .HasConversion<RoleConverter, RoleComparer>()
            .HasColumnName("role")
            .HasColumnType(ColumnType.VarChar(Role.MaxLength))
            .IsRequired();

        builder
            .Property(user => user.CreatedOn)
            .HasConversion<CreatedOnConverter, CreatedOnComparer>()
            .HasColumnName("created_at")
            .HasColumnType(ColumnType.TimestampWithTimeZone())
            .IsRequired();

        builder
            .Property(user => user.IsDeleted)
            .HasColumnName("is_deleted")
            .HasColumnType(ColumnType.Boolean)
            .IsRequired();

        builder
            .Property(user => user.DeletedAt)
            .HasConversion<DeletedAtConverter, DeletedAtComparer>()
            .HasColumnName("deleted_at")
            .HasColumnType(ColumnType.TimestampWithTimeZone())
            .IsRequired(false);

        // Indexes for performance
        builder
            .HasIndex(user => user.UserName)
            .HasDatabaseName("IX_midjourney_user_user_name");
    }
}
