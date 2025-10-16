using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class migration_init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "midjourney_styles",
                schema: "public",
                columns: table => new
                {
                    name = table.Column<string>(type: "varchar(150)", nullable: false),
                    type = table.Column<string>(type: "varchar(30)", nullable: false),
                    description = table.Column<string>(type: "varchar(500)", nullable: true),
                    tags = table.Column<string[]>(type: "Text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_midjourney_styles", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "midjourney_versions",
                schema: "public",
                columns: table => new
                {
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameter = table.Column<string>(type: "varchar(100)", nullable: false),
                    release_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "Text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_midjourney_versions", x => x.version);
                });

            migrationBuilder.CreateTable(
                name: "midjourney_prompt_history",
                schema: "public",
                columns: table => new
                {
                    history_id = table.Column<Guid>(type: "Uuid", nullable: false),
                    prompt = table.Column<string>(type: "varchar(1000)", nullable: false),
                    properties = table.Column<string>(type: "varchar(10)", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_midjourney_prompt_history", x => x.history_id);
                    table.ForeignKey(
                        name: "FK_midjourney_prompt_history_midjourney_versions_properties",
                        column: x => x.properties,
                        principalSchema: "public",
                        principalTable: "midjourney_versions",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "midjourney_properties",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "Text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "Text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_midjourney_properties", x => new { x.property_name, x.version });
                    table.ForeignKey(
                        name: "FK_midjourney_properties_midjourney_versions_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "midjourney_versions",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "midjourney_style_example_links",
                schema: "public",
                columns: table => new
                {
                    link = table.Column<string>(type: "varchar(200)", nullable: false),
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
                        name: "FK_midjourney_style_example_links_midjourney_versions_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "midjourney_versions",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MidjourneyPromptHistoryMidjourneyStyle",
                schema: "public",
                columns: table => new
                {
                    MidjourneyPromptHistoriesHistoryId = table.Column<Guid>(type: "Uuid", nullable: false),
                    MidjourneyStylesStyleName = table.Column<string>(type: "varchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MidjourneyPromptHistoryMidjourneyStyle", x => new { x.MidjourneyPromptHistoriesHistoryId, x.MidjourneyStylesStyleName });
                    table.ForeignKey(
                        name: "FK_MidjourneyPromptHistoryMidjourneyStyle_midjourney_prompt_hi~",
                        column: x => x.MidjourneyPromptHistoriesHistoryId,
                        principalSchema: "public",
                        principalTable: "midjourney_prompt_history",
                        principalColumn: "history_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MidjourneyPromptHistoryMidjourneyStyle_midjourney_styles_Mi~",
                        column: x => x.MidjourneyStylesStyleName,
                        principalSchema: "public",
                        principalTable: "midjourney_styles",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_midjourney_prompt_history_prompt",
                schema: "public",
                table: "midjourney_prompt_history",
                column: "prompt");

            migrationBuilder.CreateIndex(
                name: "IX_midjourney_prompt_history_properties",
                schema: "public",
                table: "midjourney_prompt_history",
                column: "properties");

            migrationBuilder.CreateIndex(
                name: "IX_midjourney_properties_property_name",
                schema: "public",
                table: "midjourney_properties",
                column: "property_name");

            migrationBuilder.CreateIndex(
                name: "IX_midjourney_properties_version",
                schema: "public",
                table: "midjourney_properties",
                column: "version");

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

            migrationBuilder.CreateIndex(
                name: "IX_midjourney_styles_tags",
                schema: "public",
                table: "midjourney_styles",
                column: "tags");

            migrationBuilder.CreateIndex(
                name: "IX_midjourney_styles_type",
                schema: "public",
                table: "midjourney_styles",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_MidjourneyPromptHistoryMidjourneyStyle_MidjourneyStylesStyl~",
                schema: "public",
                table: "MidjourneyPromptHistoryMidjourneyStyle",
                column: "MidjourneyStylesStyleName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "midjourney_properties",
                schema: "public");

            migrationBuilder.DropTable(
                name: "midjourney_style_example_links",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MidjourneyPromptHistoryMidjourneyStyle",
                schema: "public");

            migrationBuilder.DropTable(
                name: "midjourney_prompt_history",
                schema: "public");

            migrationBuilder.DropTable(
                name: "midjourney_styles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "midjourney_versions",
                schema: "public");
        }
    }
}
