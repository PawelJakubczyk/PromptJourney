using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class migration_add_el_index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_midjourney_style_example_links_type",
                schema: "public",
                table: "midjourney_style_example_links",
                newName: "IX_midjourney_style_example_links_style_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_midjourney_style_example_links_style_name",
                schema: "public",
                table: "midjourney_style_example_links",
                newName: "IX_midjourney_style_example_links_type");
        }
    }
}