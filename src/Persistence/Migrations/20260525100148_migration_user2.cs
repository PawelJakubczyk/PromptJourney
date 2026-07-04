using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class migration_user2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "public",
                table: "midjourney_user",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "public",
                table: "midjourney_user",
                type: "Boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "public",
                table: "midjourney_prompt_history",
                type: "Uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_midjourney_prompt_history_UserId",
                schema: "public",
                table: "midjourney_prompt_history",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_midjourney_prompt_history_midjourney_user_UserId",
                schema: "public",
                table: "midjourney_prompt_history",
                column: "UserId",
                principalSchema: "public",
                principalTable: "midjourney_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_midjourney_prompt_history_midjourney_user_UserId",
                schema: "public",
                table: "midjourney_prompt_history");

            migrationBuilder.DropIndex(
                name: "IX_midjourney_prompt_history_UserId",
                schema: "public",
                table: "midjourney_prompt_history");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                schema: "public",
                table: "midjourney_user");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "public",
                table: "midjourney_user");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "public",
                table: "midjourney_prompt_history");
        }
    }
}
