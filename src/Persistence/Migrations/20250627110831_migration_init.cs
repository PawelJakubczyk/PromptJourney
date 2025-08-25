using System;
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
                    type = table.Column<string>(type: "varchar(100)", nullable: false),
                    description = table.Column<string>(type: "varchar(800)", nullable: true),
                    tags = table.Column<string[]>(type: "text[]", nullable: true),
                    example_links = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_midjourney_styles", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "version_master",
                schema: "public",
                columns: table => new
                {
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameter = table.Column<string>(type: "varchar(15)", nullable: false),
                    release_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_master", x => x.version);
                });

            migrationBuilder.CreateTable(
                name: "midjourney_prompt_history",
                schema: "public",
                columns: table => new
                {
                    history_id = table.Column<Guid>(type: "uuid", nullable: false),
                    prompt = table.Column<string>(type: "varchar(1000)", nullable: false),
                    properties = table.Column<string>(type: "varchar(7)", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_midjourney_prompt_history", x => x.history_id);
                    table.ForeignKey(
                        name: "FK_midjourney_prompt_history_version_master_properties",
                        column: x => x.properties,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "version_1",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_1", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_1_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_2",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_2", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_2_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_3",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_3", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_3_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_4",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_4", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_4_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_5",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_5", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_5_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_5_1",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_5_1", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_5_1_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_5_2",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_5_2", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_5_2_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_6",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_6", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_6_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_6_1",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_6_1", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_6_1_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_7",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_7", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_7_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_niji_4",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_niji_4", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_niji_4_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_niji_5",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_niji_5", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_niji_5_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "version_niji_6",
                schema: "public",
                columns: table => new
                {
                    property_name = table.Column<string>(type: "varchar(25)", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", nullable: false),
                    parameters = table.Column<string[]>(type: "text[]", nullable: false),
                    default_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    min_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    max_value = table.Column<string>(type: "varchar(50)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_niji_6", x => x.property_name);
                    table.ForeignKey(
                        name: "FK_version_niji_6_version_master_version",
                        column: x => x.version,
                        principalSchema: "public",
                        principalTable: "version_master",
                        principalColumn: "version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MidjourneyPromptHistoryMidjourneyStyle",
                schema: "public",
                columns: table => new
                {
                    MidjourneyPromptHistoriesHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    MidjourneyStylesName = table.Column<string>(type: "varchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MidjourneyPromptHistoryMidjourneyStyle", x => new { x.MidjourneyPromptHistoriesHistoryId, x.MidjourneyStylesName });
                    table.ForeignKey(
                        name: "FK_MidjourneyPromptHistoryMidjourneyStyle_midjourney_prompt_hi~",
                        column: x => x.MidjourneyPromptHistoriesHistoryId,
                        principalSchema: "public",
                        principalTable: "midjourney_prompt_history",
                        principalColumn: "history_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MidjourneyPromptHistoryMidjourneyStyle_midjourney_styles_Mi~",
                        column: x => x.MidjourneyStylesName,
                        principalSchema: "public",
                        principalTable: "midjourney_styles",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_midjourney_prompt_history_properties",
                schema: "public",
                table: "midjourney_prompt_history",
                column: "properties");

            migrationBuilder.CreateIndex(
                name: "IX_MidjourneyPromptHistoryMidjourneyStyle_MidjourneyStylesName",
                schema: "public",
                table: "MidjourneyPromptHistoryMidjourneyStyle",
                column: "MidjourneyStylesName");

            migrationBuilder.CreateIndex(
                name: "IX_version_1_version",
                schema: "public",
                table: "version_1",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_2_version",
                schema: "public",
                table: "version_2",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_3_version",
                schema: "public",
                table: "version_3",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_4_version",
                schema: "public",
                table: "version_4",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_5_version",
                schema: "public",
                table: "version_5",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_5_1_version",
                schema: "public",
                table: "version_5_1",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_5_2_version",
                schema: "public",
                table: "version_5_2",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_6_version",
                schema: "public",
                table: "version_6",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_6_1_version",
                schema: "public",
                table: "version_6_1",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_7_version",
                schema: "public",
                table: "version_7",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_niji_4_version",
                schema: "public",
                table: "version_niji_4",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_niji_5_version",
                schema: "public",
                table: "version_niji_5",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "IX_version_niji_6_version",
                schema: "public",
                table: "version_niji_6",
                column: "version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MidjourneyPromptHistoryMidjourneyStyle",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_1",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_2",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_3",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_4",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_5",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_5_1",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_5_2",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_6",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_6_1",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_7",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_niji_4",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_niji_5",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_niji_6",
                schema: "public");

            migrationBuilder.DropTable(
                name: "midjourney_prompt_history",
                schema: "public");

            migrationBuilder.DropTable(
                name: "midjourney_styles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "version_master",
                schema: "public");
        }
    }
}
