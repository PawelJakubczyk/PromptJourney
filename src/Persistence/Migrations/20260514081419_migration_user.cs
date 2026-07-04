using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class migration_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "midjourney_user",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "Uuid", nullable: false),
                    user_name = table.Column<string>(type: "varchar(100)", nullable: false),
                    email = table.Column<string>(type: "varchar(320)", nullable: false),
                    password_hash = table.Column<string>(type: "varchar(512)", nullable: false),
                    role = table.Column<string>(type: "varchar(10)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_midjourney_user", x => x.user_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_midjourney_user_user_name",
                schema: "public",
                table: "midjourney_user",
                column: "user_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "midjourney_user",
                schema: "public");
        }
    }
}
