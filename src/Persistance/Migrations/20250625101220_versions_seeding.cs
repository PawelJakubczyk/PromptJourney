using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class versions_seeding : Migration
    {

        public string[] textArray(params string[] text)
        {
            return text;
        }

        public DateTime timestamp(int year, int month, int day)
        {
            return new DateTime(year, month, day).ToUniversalTime();
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed data for version_master table with parameter column
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_master",
                columns: ["version", "parameter", "release_date", "description"],
                values: new object[,]
                {
                    { "1", "--v 1", timestamp(2022, 2, 1), "Initial Midjourney version" },
                    { "2", "--v 2", timestamp(2022, 4, 15), "Improved quality and coherence" },
                    { "3", "--v 3", timestamp(2022, 7, 25), "Better composition and aesthetics" },
                    { "4", "--v 4", timestamp(2022, 11, 5), "Significantly improved image quality and coherence" },
                    { "5", "--v 5", timestamp(2023, 3, 15), "New aesthetic direction with enhanced details" },
                    { "5.1", "--v 5.1", timestamp(2023, 5, 4), "Improved handling of text and faces" },
                    { "5.2", "--v 5.2", timestamp(2023, 6, 22), "Enhanced prompt following with better composition" },
                    { "6", "--v 6", timestamp(2023, 11, 15), "Major improvement in artistic quality and coherence" },
                    { "6.1", "--v 6.1", timestamp(2024, 2, 20), "Further enhancements in detail and realism" },
                    { "7", "--v 7", timestamp(2025, 5, 15), "Latest generation with advanced detail handling" },
                    { "niji 4", "--niji 4", timestamp(2022, 12, 10), "Anime-focused model with detailed styles" },
                    { "niji 5", "--niji 5", timestamp(2023, 7, 18), "Improved anime rendering with better colors" },
                    { "niji 6", "--niji 6", timestamp(2024, 4, 1), "Enhanced anime styling with better character design" }
                });

            // Version 1 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_1",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "1", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "1", textArray("--q", "--quality"), "1", "0.25", "2", "Generation quality setting" },
                    { "seed", "1", textArray("--seed"), null, "0", "4294967295", "Seed number for reproducible results" },
                    { "no", "1", textArray("--no"), null, null, null, "Negative prompting to exclude concepts" },
                    { "stop", "1", textArray("--stop"), "100", "10", "100", "Stop generation at percentage" },
                    { "upscale", "1", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "visibility", "2", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "performance", "1", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 2 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_2",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "2", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "2", textArray("--q", "--quality"), "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "2", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "2", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "2", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "chaos", "2", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "2", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "2", textArray("--raw"), null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "2", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "2", textArray("--iw"), "1", "0.5", "2", "Relative importance of image prompt vs. text" },
                    { "weird", "2", textArray("--weird", "--w"), "0", "0", "3000", "Level of strange/unconventional output" },
                    { "styleRef", "2", textArray("--sref"), null, null, null, "Use style reference image for output guidance" },
                    { "visibility", "2", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "2", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "2", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });



            // Version 3 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_3",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "3", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "3", textArray("--q", "--quality"), "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "3", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "3", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "3", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "3", textArray("--s", "--stylize"), "30000", "625", "60000", "Strength of artistic style" },
                    { "chaos", "3", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "3", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "3", textArray("--raw"), null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "3", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "3", textArray("--iw"), "1", "0.5", "2", "Relative importance of image prompt vs. text" },
                    { "styleReference", "3", textArray("--sref"), null, null, null, "Use style reference image for output guidance" },
                    { "weird", "3", textArray("--weird", "--w"), "0", "0", "3000", "Level of unconventional creativity" },
                    { "visibility", "3", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "3", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "3", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 4 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_4",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "4", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "4", textArray("--q", "--quality"), "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "4", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "4", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "4", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "4", textArray("--s", "--stylize"), "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "4", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "4", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "4", textArray("--raw"), null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "4", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "4", textArray("--iw"), "1", "0.5", "2", "Relative importance of image prompt vs. text" },
                    { "weird", "4", textArray("--weird", "--w"), "0", "0", "3000", "Level of unconventional creativity" },
                    { "oref", "4", textArray("--oref"), null, null, null, "Omni‑reference (image/person likeness)" },
                    { "personalization", "4", textArray("--profile", "--p"), null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "4", textArray("--sref"), null, null, null, "Use style reference image" },
                    { "visibility", "4", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "4", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "4", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 5 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_5",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "5", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "5", textArray("--q", "--quality"), "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "5", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "5", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "5", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "5", textArray("--s", "--stylize"), "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "5", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "5", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "5", textArray("--raw", "--style raw"), null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "5", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "5", textArray("--iw"), "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "styleWeight", "5", textArray("--sw"), "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "5", textArray("--cw"), "100", "0", "100", "Strength of character reference image" },
                    { "weird", "5", textArray("--weird", "--w"), "0", "0", "3000", "Level of unconventional creativity" },
                    { "omniReference", "5", textArray("--oref"), null, null, null, "Omni‑reference (image/person likeness)" },
                    { "profile", "5", textArray("--profile", "--p"), null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "5", textArray("--sref"), null, null, null, "Use style reference image" },
                    { "video", "5", textArray("--video"), null, null, null, "Generate grid creation video" },
                    { "visibility", "5", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "5", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "5", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });


            // Version 5.1 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_5_1",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "5.1", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "5.1", textArray("--q", "--quality"), "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "5.1", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "5.1", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "5.1", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "5.1", textArray("--s", "--stylize"), "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "    ", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "5.1", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "5.1", textArray("--raw", "--style raw"), null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "5.1", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "5.1", textArray("--iw"), "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "styleWeight", "5.1", textArray("--sw"), "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "5.1", textArray("--cw"), "100", "0", "100", "Strength of character reference image" },
                    { "weird", "5.1", textArray("--weird", "--w"), "0", "0", "3000", "Level of unconventional creativity" },
                    { "omniReference", "5.1", textArray("--oref"), null, null, null, "Omni‑reference (image/person likeness)" },
                    { "profile", "5.1", textArray("--profile", "--p"), null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "5.1", textArray("--sref"), null, null, null, "Use style reference image" },
                    { "video", "5.1", textArray("--video"), null, null, null, "Generate grid creation video" },
                    { "visibility", "5.1", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "5.1", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "5.1", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 5.2 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_5_2",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "5.2", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "5.2", textArray("--q", "--quality"), "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "5.2", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "5.2", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "5.2", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "5.2", textArray("--s", "--stylize"), "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "5.2", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "5.2", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "5.2", textArray("--raw", "--style raw"), null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "5.2", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "5.2", textArray("--iw"), "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "styleWeight", "5.2", textArray("--sw"), "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "5.2", textArray("--cw"), "100", "0", "100", "Strength of character reference image" },
                    { "weird", "5.2", textArray("--weird", "--w"), "0", "0", "3000", "Level of unconventional creativity" },
                    { "omniReference", "5.2", textArray("--oref"), null, null, null, "Omni‑reference (image/person likeness)" },
                    { "profile", "5.2", textArray("--profile", "--p"), null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "5.2", textArray("--sref"), null, null, null, "Use style reference image" },
                    { "video", "5.2", textArray("--video"), null, null, null, "Generate grid creation video" },
                    { "visibility", "5.2", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "5.2", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "5.2", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 6 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_6",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "6", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "6", textArray("--q", "--quality"), "1", "0.25", "0.5", "Rendering quality/time" },
                    { "seed", "6", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "6", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "6", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "6", textArray("--s", "--stylize"), "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "6", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "6", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "6", textArray("--raw", "--style raw"), null, null, null, "Raw Mode: minimal styling / photorealistic" },
                    { "tile", "6", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "styleWeight", "6", textArray("--sw"), "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "6", textArray("--cw"), "100", "0", "100", "Strength of character reference image" },
                    { "weird", "6", textArray("--weird", "--w"), "0", "0", "3000", "Level of unconventional creativity" },
                    { "imageWeight", "6", textArray("--iw"), "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "imageStyleRandom", "6", textArray("--style random"), null, null, null, "Apply a random base style" },
                    { "draft", "6", textArray("--draft"), null, null, null, "Generate draft images at lower GPU cost" },
                    { "visibility", "6", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "6", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "6", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 6.1 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_6_1",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "6.1", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "6.1", textArray("--q", "--quality"), "1", "0.25", "0.5", "Rendering quality/time" },
                    { "seed", "6.1", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "6.1", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "6.1", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "6.1", textArray("--s", "--stylize"), "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "6.1", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "6.1", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "6.1", textArray("--raw", "--style raw"), null, null, null, "Raw Mode: minimal styling / photorealistic" },
                    { "tile", "6.1", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "styleWeight", "6.1", textArray("--sw"), "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "6.1", textArray("--cw"), "100", "0", "100", "Strength of character reference image" },
                    { "weird", "6.1", textArray("--weird", "--w"), "0", "0", "3000", "Level of unconventional creativity" },
                    { "imageWeight", "6.1", textArray("--iw"), "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "imageStyleRandom", "6.1  ", textArray("--style random"), null, null, null, "Apply a random base style" },
                    { "draft", "6.1", textArray("--draft"), null, null, null, "Generate draft images at lower GPU cost" },
                    { "visibility", "6.1", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "6.1", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "6.1", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });




            // Version 7 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_7",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "7", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "7", textArray("--q", "--quality"), "1", "0.5", "4", "Rendering quality/time (0.5, 1, 2, 4*) – *Q4 is experimental, not compatible with oref" },
                    { "seed", "7", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "7", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "7", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "7", textArray("--s", "--stylize"), "100", "0", "1000", "Artistic flair strength" },
                    { "chaos", "7", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness" },
                    { "repeat", "7", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "7", textArray("--raw"), null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "7", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "7", textArray("--iw"), "1", "0", "3", "Influence of image prompt" },
                    { "weird", "7", textArray("--weird", "--w"), "0", "0", "3000", "Experimental/unconventional creativity" },
                    { "niji", "7", textArray("--niji"), null, null, null, "Switch to anime‑focused model" },
                    { "draft", "7", textArray("--draft"), null, null, null, "Draft Mode: fast, low‑cost prototyping" },
                    { "omeComplex", "7", textArray("--oref"), null, null, null, "Omni‑reference (image/person likeness)" },
                    { "styleReference", "7", textArray("--sref"), null, null, null, "Style reference image" },
                    { "styleWeight", "7", textArray("--sw"), "100", "0", "1000", "Strength of style reference" },
                    { "versionFlag", "7", textArray("--v", "--version"), "7", null, null, "Explicit version flag" },
                    { "characterReference", "7", textArray("--cref"), null, null, null, "Content reference image" },
                    { "stealth", "7", textArray("--stealth"), null, null, null, "Private/unpublished job" },
                    { "public", "7", textArray("--public"), null, null, null, "Force public job" },
                    { "fast", "7", textArray("--fast"), null, null, null, "Fast Mode: high‑speed rendering" },
                    { "relax", "7", textArray("--relax"), null, null, null, "Relax Mode: low‑priority queue" },
                    { "turbo", "7", textArray("--turbo"), null, null, null, "Turbo Mode: fastest rendering" },
                    { "exp", "7", textArray("--exp"), null, null, null, "Experimental aesthetics mode" },
                    { "sv", "7", textArray("--sv"), "7", "4", "7", "Style‑version for sref codes" },
                    { "profile", "7", textArray("--style", "--profile"), null, null, null, "Base style for the generation" },
                    { "blend", "7", textArray("--blend"), null, null, null, "Blend multiple images together" },
                    { "visibility", "7", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "7", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "7", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }

                });


            // Version niji_4 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_niji_4",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "niji4", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "niji4", textArray("--q", "--quality"), "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "niji4", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "niji4", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "niji4", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "niji4", textArray("--s", "--stylize"), "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "niji4", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "niji4", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "niji4", textArray("--raw"), null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "niji4", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "niji4", textArray("--iw"), "1", "0.5", "2", "Relative importance of image prompt vs. text" },
                    { "weird", "niji4", textArray("--weird", "--w"), "0", "0", "3000", "Level of unconventional creativity" },
                    { "oref", "niji4", textArray("--oref"), null, null, null, "Omni‑reference (image/person likeness)" },
                    { "personalization", "niji4", textArray("--profile", "--p"), null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "niji4", textArray("--sref"), null, null, null, "Use style reference image" },
                    { "visibility", "niji4", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "niji4", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "niji4", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version niji_5 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_niji_5",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "niji5", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "niji5", textArray("--q", "--quality"), "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "niji5", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "niji5", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "niji5", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "niji5", textArray("--s", "--stylize"), "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "niji5", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "niji5", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "niji5", textArray("--raw", "--style raw"), null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "niji5", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "niji5", textArray("--iw"), "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "styleWeight", "niji5", textArray("--sw"), "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "niji5", textArray("--cw"), "100", "0", "100", "Strength of character reference image" },
                    { "weird", "niji5", textArray("--weird", "--w"), "0", "0", "3000", "Level of unconventional creativity" },
                    { "omniReference", "niji5", textArray("--oref"), null, null, null, "Omni‑reference (image/person likeness)" },
                    { "profile", "niji5", textArray("--profile", "--p"), null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "niji5", textArray("--sref"), null, null, null, "Use style reference image" },
                    { "video", "niji5", textArray("--video"), null, null, null, "Generate grid creation video" },
                    { "visibility", "niji5", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "niji5", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "niji5", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version niji_6 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_niji_6",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "niji6", textArray("--ar", "--aspect"), "1:1", null, null, "Image aspect ratio" },
                    { "quality", "niji6", textArray("--q", "--quality"), "1", "0.25", "0.5", "Rendering quality/time" },
                    { "seed", "niji6", textArray("--seed"), null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "niji6", textArray("--no"), null, null, null, "Exclude specified elements" },
                    { "stop", "niji6", textArray("--stop"), "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "niji6", textArray("--s", "--stylize"), "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "niji6", textArray("--c", "--chaos"), "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "niji6", textArray("--r", "--repeat"), "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "niji6", textArray("--raw", "--style raw"), null, null, null, "Raw Mode: minimal styling / photorealistic" },
                    { "tile", "niji6", textArray("--tile"), null, null, null, "Seamless repeating pattern" },
                    { "styleWeight", "niji6", textArray("--sw"), "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "niji6", textArray("--cw"), "100", "0", "100", "Strength of character reference image" },
                    { "weird", "niji6", textArray("--weird", "--w"), "0", "0", "3000", "Level of unconventional creativity" },
                    { "imageWeight", "niji6", textArray("--iw"), "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "imageStyleRandom", "niji6", textArray("--style random"), null, null, null, "Apply a random base style" },
                    { "draft", "niji6", textArray("--draft"), null, null, null, "Generate draft images at lower GPU cost" },
                    { "visibility", "niji6", textArray("--public", "--stealth"), null, null, null, "Force generation to be public / stelth" },
                    { "upscale", "niji6", textArray("--upbeta", "--uplight"), null, null, null, "Use the light/beta upscaler" },
                    { "performance", "niji6", textArray("--relax", "--fast", "--turbo"), null, null, null, "Use relaxed / fast / turbo generation mode" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove seeded data from version tables
            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_niji_6",
                keyColumn: "PropertyName",
                keyValues: ["aspectRatio", "quality", "seed", "no", "stylize", "stop", "profile", "niji", "characterReference", "styleReference"]);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_niji_5",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "profile", "niji" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_niji_4",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "niji" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_7",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "imageWeight", "chaos", "weird", "tile",
                           "profile", "repeat", "characterReference", "styleReference", "blend", "raw" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_6_1",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "imageWeight", "chaos", "weird", "tile",
                           "profile", "repeat", "characterReference", "styleReference" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_6",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "imageWeight", "chaos", "weird", "tile",
                           "profile", "repeat" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_5_2",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "imageWeight", "chaos", "weird", "tile", "profile" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_5_1",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "imageWeight", "chaos", "weird", "tile", "profile" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_5",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "imageWeight", "chaos", "weird", "tile" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_4",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "imageWeight", "chaos" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_3",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "imageWeight" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_2",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_1",
                keyColumn: "PropertyName",
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stop", "upscale", "performance" });

            // Remove seeded data from version_master
            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_master",
                keyColumn: "version",
                keyValues: new string[] { "1", "2", "3", "4", "5", "5.1", "5.2", "6", "6.1", "7", "niji 4", "niji 5", "niji 6" });
        }
    }
}

//("--ar", "--aspect")
//("--q", "--quality")
//("--relax", "--fast", "--turbo")
//("--public", "--stealth")
//("--s", "--stylize")    3=>
