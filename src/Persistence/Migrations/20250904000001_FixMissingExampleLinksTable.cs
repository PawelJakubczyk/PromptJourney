using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations;

public partial class FixMissingExampleLinksTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Remove example_links column from midjourney_styles table
        migrationBuilder.DropColumn(
            name: "example_links",
            schema: "public",
            table: "midjourney_styles");

        // Create the missing midjourney_style_example_links table
        migrationBuilder.CreateTable(
            name: "midjourney_style_example_links",
            schema: "public",
            columns: table => new
            {
                link = table.Column<string>(type: "varchar(500)", nullable: false),
                style_name = table.Column<string>(type: "varchar(150)", nullable: false),
                version = table.Column<string>(type: "varchar(10)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_midjourney_style_example_links", x => new { x.link, x.style_name, x.version });
                table.ForeignKey(
                    name: "FK_midjourney_style_example_links_midjourney_styles_style_name",
                    column: x => x.style_name,
                    principalSchema: "public",
                    principalTable: "midjourney_styles",
                    principalColumn: "name",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_midjourney_style_example_links_version_master_version",
                    column: x => x.version,
                    principalSchema: "public",
                    principalTable: "version_master",
                    principalColumn: "version",
                    onDelete: ReferentialAction.Cascade);
            });

        // Create indexes
        migrationBuilder.CreateIndex(
            name: "IX_midjourney_style_example_links_style_name",
            schema: "public",
            table: "midjourney_style_example_links",
            column: "style_name");

        migrationBuilder.CreateIndex(
            name: "IX_midjourney_style_example_links_version",
            schema: "public",
            table: "midjourney_style_example_links",
            column: "version");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop the example links table
        migrationBuilder.DropTable(
            name: "midjourney_style_example_links",
            schema: "public");

        // Add back example_links column to midjourney_styles
        migrationBuilder.AddColumn<string[]>(
            name: "example_links",
            schema: "public", 
            table: "midjourney_styles",
            type: "text[]",
            nullable: false,
            defaultValue: new string[0]);
    }
}