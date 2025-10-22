using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class migration_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_midjourney_style_example_links",
                schema: "public",
                table: "midjourney_style_example_links");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "public",
                table: "midjourney_style_example_links",
                type: "Uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_midjourney_style_example_links",
                schema: "public",
                table: "midjourney_style_example_links",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_midjourney_style_example_links",
                schema: "public",
                table: "midjourney_style_example_links");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "public",
                table: "midjourney_style_example_links");

            migrationBuilder.AddPrimaryKey(
                name: "PK_midjourney_style_example_links",
                schema: "public",
                table: "midjourney_style_example_links",
                columns: ["link", "style_name", "version"]);
        }
    }
}
