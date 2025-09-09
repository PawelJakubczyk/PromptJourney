using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class migration_seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed data for version_master table
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_master",
                columns: ["version", "parameter", "release_date", "description"],
                values: new object[,]
                {
                    { "1", "--v 1", new DateTime(2022, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Initial Midjourney version" },
                    { "2", "--v 2", new DateTime(2022, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Improved quality and coherence" },
                    { "3", "--v 3", new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Better composition and aesthetics" },
                    { "4", "--v 4", new DateTime(2022, 11, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Significantly improved image quality and coherence" },
                    { "5", "--v 5", new DateTime(2023, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "New aesthetic direction with enhanced details" },
                    { "5.1", "--v 5.1", new DateTime(2023, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Improved handling of text and faces" },
                    { "5.2", "--v 5.2", new DateTime(2023, 6, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Enhanced prompt following with better composition" },
                    { "6", "--v 6", new DateTime(2023, 11, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Major improvement in artistic quality and coherence" },
                    { "6.1", "--v 6.1", new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Further enhancements in detail and realism" },
                    { "7", "--v 7", new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Latest generation with advanced detail handling" },
                    { "niji 4", "--niji 4", new DateTime(2022, 12, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Anime-focused model with detailed styles" },
                    { "niji 5", "--niji 5", new DateTime(2023, 7, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Improved anime rendering with better colors" },
                    { "niji 6", "--niji 6", new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Enhanced anime styling with better character design" }
                });

            // Version 1 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_1",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "1", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "1", new string[] { "--q", "--quality" }, "1", "0.25", "2", "Generation quality setting" },
                    { "seed", "1", new string[] { "--seed" }, null, "0", "4294967295", "Seed number for reproducible results" },
                    { "no", "1", new string[] { "--no" }, null, null, null, "Negative prompting to exclude concepts" },
                    { "stop", "1", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percentage" },
                    { "upscale", "1", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "visibility", "1", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "performance", "1", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 2 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_2",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "2", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "2", new string[] { "--q", "--quality" }, "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "2", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "2", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "2", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "chaos", "2", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "2", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "2", new string[] { "--raw" }, null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "2", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "2", new string[] { "--iw" }, "1", "0.5", "2", "Relative importance of image prompt vs. text" },
                    { "weird", "2", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of strange/unconventional output" },
                    { "styleReference", "2", new string[] { "--sref" }, null, null, null, "Use style reference image for output guidance" },
                    { "visibility", "2", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "2", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "2", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 3 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_3",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "3", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "3", new string[] { "--q", "--quality" }, "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "3", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "3", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "3", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "3", new string[] { "--s", "--stylize" }, "30000", "625", "60000", "Strength of artistic style" },
                    { "chaos", "3", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "3", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "3", new string[] { "--raw" }, null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "3", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "3", new string[] { "--iw" }, "1", "0.5", "2", "Relative importance of image prompt vs. text" },
                    { "styleReference", "3", new string[] { "--sref" }, null, null, null, "Use style reference image for output guidance" },
                    { "weird", "3", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of unconventional creativity" },
                    { "visibility", "3", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "3", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "3", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 4 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_4",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "4", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "4", new string[] { "--q", "--quality" }, "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "4", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "4", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "4", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "4", new string[] { "--s", "--stylize" }, "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "4", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "4", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "4", new string[] { "--raw" }, null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "4", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "4", new string[] { "--iw" }, "1", "0.5", "2", "Relative importance of image prompt vs. text" },
                    { "weird", "4", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of unconventional creativity" },
                    { "oref", "4", new string[] { "--oref" }, null, null, null, "Omni‑reference (image/person likeness)" },
                    { "personalization", "4", new string[] { "--profile", "--p" }, null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "4", new string[] { "--sref" }, null, null, null, "Use style reference image" },
                    { "visibility", "4", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "4", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "4", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 5 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_5",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "5", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "5", new string[] { "--q", "--quality" }, "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "5", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "5", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "5", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "5", new string[] { "--s", "--stylize" }, "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "5", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "5", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "5", new string[] { "--raw", "--style raw" }, null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "5", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "5", new string[] { "--iw" }, "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "styleWeight", "5", new string[] { "--sw" }, "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "5", new string[] { "--cw" }, "100", "0", "100", "Strength of character reference image" },
                    { "weird", "5", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of unconventional creativity" },
                    { "omniReference", "5", new string[] { "--oref" }, null, null, null, "Omni‑reference (image/person likeness)" },
                    { "profile", "5", new string[] { "--profile", "--p" }, null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "5", new string[] { "--sref" }, null, null, null, "Use style reference image" },
                    { "video", "5", new string[] { "--video" }, null, null, null, "Generate grid creation video" },
                    { "visibility", "5", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "5", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "5", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 5.1 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_5_1",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "5.1", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "5.1", new string[] { "--q", "--quality" }, "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "5.1", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "5.1", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "5.1", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "5.1", new string[] { "--s", "--stylize" }, "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "5.1", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "5.1", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "5.1", new string[] { "--raw", "--style raw" }, null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "5.1", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "5.1", new string[] { "--iw" }, "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "styleWeight", "5.1", new string[] { "--sw" }, "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "5.1", new string[] { "--cw" }, "100", "0", "100", "Strength of character reference image" },
                    { "weird", "5.1", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of unconventional creativity" },
                    { "omniReference", "5.1", new string[] { "--oref" }, null, null, null, "Omni‑reference (image/person likeness)" },
                    { "profile", "5.1", new string[] { "--profile", "--p" }, null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "5.1", new string[] { "--sref" }, null, null, null, "Use style reference image" },
                    { "video", "5.1", new string[] { "--video" }, null, null, null, "Generate grid creation video" },
                    { "visibility", "5.1", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "5.1", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "5.1", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 5.2 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_5_2",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "5.2", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "5.2", new string[] { "--q", "--quality" }, "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "5.2", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "5.2", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "5.2", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "5.2", new string[] { "--s", "--stylize" }, "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "5.2", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "5.2", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "5.2", new string[] { "--raw", "--style raw" }, null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "5.2", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "5.2", new string[] { "--iw" }, "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "styleWeight", "5.2", new string[] { "--sw" }, "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "5.2", new string[] { "--cw" }, "100", "0", "100", "Strength of character reference image" },
                    { "weird", "5.2", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of unconventional creativity" },
                    { "omniReference", "5.2", new string[] { "--oref" }, null, null, null, "Omni‑reference (image/person likeness)" },
                    { "profile", "5.2", new string[] { "--profile", "--p" }, null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "5.2", new string[] { "--sref" }, null, null, null, "Use style reference image" },
                    { "video", "5.2", new string[] { "--video" }, null, null, null, "Generate grid creation video" },
                    { "visibility", "5.2", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "5.2", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "5.2", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 6 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_6",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "6", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "6", new string[] { "--q", "--quality" }, "1", "0.25", "0.5", "Rendering quality/time" },
                    { "seed", "6", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "6", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "6", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "6", new string[] { "--s", "--stylize" }, "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "6", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "6", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "6", new string[] { "--raw", "--style raw" }, null, null, null, "Raw Mode: minimal styling / photorealistic" },
                    { "tile", "6", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "styleWeight", "6", new string[] { "--sw" }, "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "6", new string[] { "--cw" }, "100", "0", "100", "Strength of character reference image" },
                    { "weird", "6", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of unconventional creativity" },
                    { "imageWeight", "6", new string[] { "--iw" }, "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "imageStyleRandom", "6", new string[] { "--style random" }, null, null, null, "Apply a random base style" },
                    { "draft", "6", new string[] { "--draft" }, null, null, null, "Generate draft images at lower GPU cost" },
                    { "visibility", "6", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "6", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "6", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 6.1 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_6_1",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "6.1", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "6.1", new string[] { "--q", "--quality" }, "1", "0.25", "0.5", "Rendering quality/time" },
                    { "seed", "6.1", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "6.1", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "6.1", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "6.1", new string[] { "--s", "--stylize" }, "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "6.1", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "6.1", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "6.1", new string[] { "--raw", "--style raw" }, null, null, null, "Raw Mode: minimal styling / photorealistic" },
                    { "tile", "6.1", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "styleWeight", "6.1", new string[] { "--sw" }, "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "6.1", new string[] { "--cw" }, "100", "0", "100", "Strength of character reference image" },
                    { "weird", "6.1", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of unconventional creativity" },
                    { "imageWeight", "6.1", new string[] { "--iw" }, "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "imageStyleRandom", "6.1", new string[] { "--style random" }, null, null, null, "Apply a random base style" },
                    { "draft", "6.1", new string[] { "--draft" }, null, null, null, "Generate draft images at lower GPU cost" },
                    { "visibility", "6.1", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "6.1", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "6.1", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version 7 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_7",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "7", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "7", new string[] { "--q", "--quality" }, "1", "0.5", "4", "Rendering quality/time (0.5, 1, 2, 4*) – *Q4 is experimental, not compatible with oref" },
                    { "seed", "7", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "7", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "7", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "7", new string[] { "--s", "--stylize" }, "100", "0", "1000", "Artistic flair strength" },
                    { "chaos", "7", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness" },
                    { "repeat", "7", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "7", new string[] { "--raw" }, null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "7", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "7", new string[] { "--iw" }, "1", "0", "3", "Influence of image prompt" },
                    { "weird", "7", new string[] { "--weird", "--w" }, "0", "0", "3000", "Experimental/unconventional creativity" },
                    { "niji", "7", new string[] { "--niji" }, null, null, null, "Switch to anime‑focused model" },
                    { "draft", "7", new string[] { "--draft" }, null, null, null, "Draft Mode: fast, low‑cost prototyping" },
                    { "omniReference", "7", new string[] { "--oref" }, null, null, null, "Omni‑reference (image/person likeness)" },
                    { "styleReference", "7", new string[] { "--sref" }, null, null, null, "Style reference image" },
                    { "styleWeight", "7", new string[] { "--sw" }, "100", "0", "1000", "Strength of style reference" },
                    { "versionFlag", "7", new string[] { "--v", "--version" }, "7", null, null, "Explicit version flag" },
                    { "characterReference", "7", new string[] { "--cref" }, null, null, null, "Content reference image" },
                    { "stealth", "7", new string[] { "--stealth" }, null, null, null, "Private/unpublished job" },
                    { "public", "7", new string[] { "--public" }, null, null, null, "Force public job" },
                    { "fast", "7", new string[] { "--fast" }, null, null, null, "Fast Mode: high‑speed rendering" },
                    { "relax", "7", new string[] { "--relax" }, null, null, null, "Relax Mode: low‑priority queue" },
                    { "turbo", "7", new string[] { "--turbo" }, null, null, null, "Turbo Mode: fastest rendering" },
                    { "exp", "7", new string[] { "--exp" }, null, null, null, "Experimental aesthetics mode" },
                    { "sv", "7", new string[] { "--sv" }, "7", "4", "7", "Style‑version for sref codes" },
                    { "profile", "7", new string[] { "--style", "--profile" }, null, null, null, "Base style for the generation" },
                    { "blend", "7", new string[] { "--blend" }, null, null, null, "Blend multiple images together" },
                    { "visibility", "7", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "7", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "7", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version niji_4 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_niji_4",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "niji 4", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "niji 4", new string[] { "--q", "--quality" }, "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "niji 4", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "niji 4", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "niji 4", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "niji 4", new string[] { "--s", "--stylize" }, "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "niji 4", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "niji 4", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "niji 4", new string[] { "--raw" }, null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "niji 4", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "niji 4", new string[] { "--iw" }, "1", "0.5", "2", "Relative importance of image prompt vs. text" },
                    { "weird", "niji 4", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of unconventional creativity" },
                    { "oref", "niji 4", new string[] { "--oref" }, null, null, null, "Omni‑reference (image/person likeness)" },
                    { "personalization", "niji 4", new string[] { "--profile", "--p" }, null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "niji 4", new string[] { "--sref" }, null, null, null, "Use style reference image" },
                    { "visibility", "niji 4", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "niji 4", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "niji 4", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version niji_5 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_niji_5",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "niji 5", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "niji 5", new string[] { "--q", "--quality" }, "1", "0.25", "2", "Rendering quality/time" },
                    { "seed", "niji 5", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "niji 5", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "niji 5", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "niji 5", new string[] { "--s", "--stylize" }, "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "niji 5", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "niji 5", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "niji 5", new string[] { "--raw", "--style raw" }, null, null, null, "Raw Mode: minimal styling" },
                    { "tile", "niji 5", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "imageWeight", "niji 5", new string[] { "--iw" }, "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "styleWeight", "niji 5", new string[] { "--sw" }, "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "niji 5", new string[] { "--cw" }, "100", "0", "100", "Strength of character reference image" },
                    { "weird", "niji 5", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of unconventional creativity" },
                    { "omniReference", "niji 5", new string[] { "--oref" }, null, null, null, "Omni‑reference (image/person likeness)" },
                    { "profile", "niji 5", new string[] { "--profile", "--p" }, null, null, null, "Use personalization profile/moodboard" },
                    { "styleReference", "niji 5", new string[] { "--sref" }, null, null, null, "Use style reference image" },
                    { "video", "niji 5", new string[] { "--video" }, null, null, null, "Generate grid creation video" },
                    { "visibility", "niji 5", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "niji 5", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "niji 5", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });

            // Version niji_6 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "properties_version_niji_6",
                columns: ["property_name", "version", "parameters", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "niji 6", new string[] { "--ar", "--aspect" }, "1:1", null, null, "Image aspect ratio" },
                    { "quality", "niji 6", new string[] { "--q", "--quality" }, "1", "0.25", "0.5", "Rendering quality/time" },
                    { "seed", "niji 6", new string[] { "--seed" }, null, "0", "4294967295", "Seed for reproducibility" },
                    { "no", "niji 6", new string[] { "--no" }, null, null, null, "Exclude specified elements" },
                    { "stop", "niji 6", new string[] { "--stop" }, "100", "10", "100", "Stop generation at percent" },
                    { "stylize", "niji 6", new string[] { "--s", "--stylize" }, "100", "0", "1000", "Strength of artistic style" },
                    { "chaos", "niji 6", new string[] { "--c", "--chaos" }, "0", "0", "100", "Variation/randomness in results" },
                    { "repeat", "niji 6", new string[] { "--r", "--repeat" }, "1", "1", "40", "Generate multiple jobs" },
                    { "raw", "niji 6", new string[] { "--raw", "--style raw" }, null, null, null, "Raw Mode: minimal styling / photorealistic" },
                    { "tile", "niji 6", new string[] { "--tile" }, null, null, null, "Seamless repeating pattern" },
                    { "styleWeight", "niji 6", new string[] { "--sw" }, "100", "0", "1000", "Strength of style reference image" },
                    { "characterWeight", "niji 6", new string[] { "--cw" }, "100", "0", "100", "Strength of character reference image" },
                    { "weird", "niji 6", new string[] { "--weird", "--w" }, "0", "0", "3000", "Level of unconventional creativity" },
                    { "imageWeight", "niji 6", new string[] { "--iw" }, "1", "0", "2", "Relative importance of image prompt vs. text" },
                    { "imageStyleRandom", "niji 6", new string[] { "--style random" }, null, null, null, "Apply a random base style" },
                    { "draft", "niji 6", new string[] { "--draft" }, null, null, null, "Generate draft images at lower GPU cost" },
                    { "visibility", "niji 6", new string[] { "--public", "--stealth" }, null, null, null, "Force generation to be public / stealth" },
                    { "upscale", "niji 6", new string[] { "--upbeta", "--uplight" }, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "niji 6", new string[] { "--relax", "--fast", "--turbo" }, null, null, null, "Use relaxed / fast / turbo generation mode" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove seeded data in reverse order (child tables first, then parent tables)

            // Remove properties data for all versions
            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_niji_6",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "styleWeight", "characterWeight", "weird", "imageWeight", "imageStyleRandom", "draft", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_niji_5",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "imageWeight", "styleWeight", "characterWeight", "weird", "omniReference", "profile", "styleReference", "video", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_niji_4",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "imageWeight", "weird", "oref", "personalization", "styleReference", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_7",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "imageWeight", "weird", "niji", "draft", "omniReference", "styleReference", "styleWeight", "versionFlag", "characterReference", "stealth", "public", "fast", "relax", "turbo", "exp", "sv", "profile", "blend", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_6_1",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "styleWeight", "characterWeight", "weird", "imageWeight", "imageStyleRandom", "draft", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_6",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "styleWeight", "characterWeight", "weird", "imageWeight", "imageStyleRandom", "draft", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_5_2",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "imageWeight", "styleWeight", "characterWeight", "weird", "omniReference", "profile", "styleReference", "video", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_5_1",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "imageWeight", "styleWeight", "characterWeight", "weird", "omniReference", "profile", "styleReference", "video", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_5",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "imageWeight", "styleWeight", "characterWeight", "weird", "omniReference", "profile", "styleReference", "video", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_4",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "imageWeight", "weird", "oref", "personalization", "styleReference", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_3",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "stylize", "chaos", "repeat", "raw", "tile", "imageWeight", "styleReference", "weird", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_2",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "chaos", "repeat", "raw", "tile", "imageWeight", "weird", "styleReference", "visibility", "upscale", "performance" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "properties_version_1",
                keyColumn: "property_name",
                keyValues: new object[] { "aspectRatio", "quality", "seed", "no", "stop", "upscale", "visibility", "performance" });

            // Remove version master data last (parent table)
            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_master",
                keyColumn: "version",
                keyValues: new object[] { "1", "2", "3", "4", "5", "5.1", "5.2", "6", "6.1", "7", "niji 4", "niji 5", "niji 6" });
        }
    }
}