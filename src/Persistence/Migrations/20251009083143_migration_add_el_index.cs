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
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE schemaname = 'public' 
                        AND tablename = 'midjourney_style_example_links' 
                        AND indexname = 'IX_midjourney_style_example_links_type'
                    ) THEN
                        DROP INDEX public.""IX_midjourney_style_example_links_type"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_midjourney_style_example_links_style_name"" 
                ON public.midjourney_style_example_links (style_name);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // W przypadku rollbacku usuń indeks
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS public.""IX_midjourney_style_example_links_style_name"";
            ");
        }
    }
}