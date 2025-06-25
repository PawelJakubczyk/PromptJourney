using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class versions_seeding : Migration
    {
        private static readonly string[] _values = ["default"];

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
                    { "1", "--v 1", new DateTime(2022, 2, 1).ToUniversalTime(), "Initial Midjourney version" },
                    { "2", "--v 2", new DateTime(2022, 4, 15).ToUniversalTime(), "Improved quality and coherence" },
                    { "3", "--v 3", new DateTime(2022, 7, 25).ToUniversalTime(), "Better composition and aesthetics" },
                    { "4", "--v 4", new DateTime(2022, 11, 5).ToUniversalTime(), "Significantly improved image quality and coherence" },
                    { "5", "--v 5", new DateTime(2023, 3, 15).ToUniversalTime(), "New aesthetic direction with enhanced details" },
                    { "5.1", "--v 5.1", new DateTime(2023, 5, 4).ToUniversalTime(), "Improved handling of text and faces" },
                    { "5.2", "--v 5.2", new DateTime(2023, 6, 22).ToUniversalTime(), "Enhanced prompt following with better composition" },
                    { "6", "--v 6", new DateTime(2023, 11, 15).ToUniversalTime(), "Major improvement in artistic quality and coherence" },
                    { "6.1", "--v 6.1", new DateTime(2024, 2, 20).ToUniversalTime(), "Further enhancements in detail and realism" },
                    { "7", "--v 7", new DateTime(2025, 5, 15).ToUniversalTime(), "Latest generation with advanced detail handling" },
                    { "niji 4", "--niji 4", new DateTime(2022, 12, 10).ToUniversalTime(), "Anime-focused model with detailed styles" },
                    { "niji 5", "--niji 5", new DateTime(2023, 7, 18).ToUniversalTime(), "Improved anime rendering with better colors" },
                    { "niji 6", "--niji 6", new DateTime(2024, 4, 1).ToUniversalTime(), "Enhanced anime styling with better character design" }
                });

            // Version 1 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_1",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "1", "--ar", "1:1", null, null, "Image aspect ratio", null },
                    { "quality", "1", "--q", "1", "0.25", "2", "Generation quality setting", null },
                    { "seed", "1", "--seed", null, "0", "4294967295", "Seed number for reproducible results", null },
                    { "no", "1", "--no", null, null, null, "Negative prompting to exclude concepts", null },
                    { "stop", "1", "--stop", "100", "10", "100", "Stop generation at percentage", null },
                    { "upscale", "1", "--upbeta, --uplight", null, null, null, "Use the light/beta upscaler", null },
                    { "performance", "1", "--relax, --fast", null, null, null, "Use relaxed / fast generation mode", null },
                });
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_1",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description"],
                values: new object[,]
                {
                    { "aspectRatio", "1", new string[] {"--ar"}, "1:1", null, null, "Image aspect ratio"},
                    { "quality", "1", new string[] {"--q", "--quality"}, "1", "0.25", "2", "Generation quality setting",},
                    { "seed", "1", new string[] {"--seed"}, null, "0", "4294967295", "Seed number for reproducible results"},
                    { "no", "1", new string[] {"--no"}, null, null, null, "Negative prompting to exclude concepts",},
                    { "stop", "1", new string[] {"--stop"}, "100", "10", "100", "Stop generation at percentage" },
                    { "upscale", "1", new string[] {"--upbeta", "--uplight"}, null, null, null, "Use the light/beta upscaler" },
                    { "performance", "1", new string[] {"--relax", "--fast"}, null, null, null, "Use relaxed / fast generation mode" }
                });

            // Version 2 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_2",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes"],
                values: new object[,]
                {
                    { "aspectRatio", "2", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default"} },
                    { "quality", "2", "--q", "1", "0.25", "2", "Generation quality setting", new string[] {"default"} },
                    { "seed", "2", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default"} },
                    { "no", "2", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default"} },
                    { "stylize", "2", "--s", "100", "0", "750", "Stylization strength setting", new string[] {"default"} },
                    { "stop", "2", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default"} }
                });

            // Version 3 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_3",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes"],
                values: new object[,]
                {
                    { "aspectRatio", "3", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default"} },
                    { "quality", "3", "--q", "1", "0.25", "2", "Generation quality setting", new string[] {"default"} },
                    { "seed", "3", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default"} },
                    { "no", "3", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default"} },
                    { "stylize", "3", "--s", "100", "0", "1000", "Stylization strength setting", new string[] {"default"} },
                    { "stop", "3", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default"} },
                    { "imageWeight", "3", "--iw", null, null, null, "Image weight for image prompting", new string[] {"default"} }
                });

            // Version 4 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_4",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes", "parameter_modes"],
                values: new object[,]
                {
                    { "aspectRatio", "4", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default", "fast", "relax"}, new string[] {"1:1", "16:9", "9:16", "4:3", "3:4", "5:4", "4:5"} },
                    { "quality", "4", "--q", "1", "0.25", "2", "Generation quality setting", new string[] {"default", "fast", "relax"}, null },
                    { "seed", "4", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default", "fast", "relax"}, null },
                    { "no", "4", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default", "fast", "relax"}, null },
                    { "stylize", "4", "--s", "100", "0", "1000", "Stylization strength setting", new string[] {"default", "fast", "relax"}, null },
                    { "stop", "4", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default", "fast", "relax"}, null },
                    { "imageWeight", "4", "--iw", null, null, null, "Image weight for image prompting", new string[] {"default", "fast", "relax"}, null },
                    { "chaos", "4", "--c", "0", "0", "100", "Chaos/randomness setting", new string[] {"default", "fast", "relax"}, null }
                });

            // Version 5 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_5",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes", "parameter_modes"],
                values: new object[,]
                {
                    { "aspectRatio", "5", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default", "fast", "relax"}, new string[] {"1:1", "16:9", "9:16", "4:3", "3:4", "5:4", "4:5", "2:3", "3:2"} },
                    { "quality", "5", "--q", "1", "0.25", "2", "Generation quality setting", new string[] {"default", "fast", "relax"}, null },
                    { "seed", "5", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default", "fast", "relax"}, null },
                    { "no", "5", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default", "fast", "relax"}, null },
                    { "stylize", "5", "--s", "100", "0", "1000", "Stylization strength setting", new string[] {"default", "fast", "relax"}, null },
                    { "stop", "5", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default", "fast", "relax"}, null },
                    { "imageWeight", "5", "--iw", null, null, null, "Image weight for image prompting", new string[] {"default", "fast", "relax"}, null },
                    { "chaos", "5", "--c", "0", "0", "100", "Chaos/randomness setting", new string[] {"default", "fast", "relax"}, null },
                    { "weird", "5", "--weird", "0", "0", "3000", "Weirdness level setting", new string[] {"default", "fast", "relax"}, null },
                    { "tile", "5", "--tile", null, null, null, "Creates seamless pattern tiles", new string[] {"default", "fast", "relax"}, null }
                });

            // Version 5.1 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_5_1",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes", "parameter_modes"],
                values: new object[,]
                {
                    { "aspectRatio", "5.1", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default", "fast", "relax"}, new string[] {"1:1", "16:9", "9:16", "4:3", "3:4", "5:4", "4:5", "2:3", "3:2"} },
                    { "quality", "5.1", "--q", "1", "0.25", "2", "Generation quality setting", new string[] {"default", "fast", "relax"}, null },
                    { "seed", "5.1", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default", "fast", "relax"}, null },
                    { "no", "5.1", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default", "fast", "relax"}, null },
                    { "stylize", "5.1", "--s", "100", "0", "1000", "Stylization strength setting", new string[] {"default", "fast", "relax"}, null },
                    { "stop", "5.1", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default", "fast", "relax"}, null },
                    { "imageWeight", "5.1", "--iw", null, null, null, "Image weight for image prompting", new string[] {"default", "fast", "relax"}, null },
                    { "chaos", "5.1", "--c", "0", "0", "100", "Chaos/randomness setting", new string[] {"default", "fast", "relax"}, null },
                    { "weird", "5.1", "--weird", "0", "0", "3000", "Weirdness level setting", new string[] {"default", "fast", "relax"}, null },
                    { "tile", "5.1", "--tile", null, null, null, "Creates seamless pattern tiles", new string[] {"default", "fast", "relax"}, null },
                    { "profile", "5.1", "--style", null, null, null, "Base style for the generation", new string[] {"default", "fast", "relax"}, new string[] {"raw", "cute", "expressive"} }
                });

            // Version 5.2 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_5_2",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes", "parameter_modes"],
                values: new object[,]
                {
                    { "aspectRatio", "5.2", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default", "fast", "relax"}, new string[] {"1:1", "16:9", "9:16", "4:3", "3:4", "5:4", "4:5", "2:3", "3:2"} },
                    { "quality", "5.2", "--q", "1", "0.25", "2", "Generation quality setting", new string[] {"default", "fast", "relax"}, null },
                    { "seed", "5.2", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default", "fast", "relax"}, null },
                    { "no", "5.2", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default", "fast", "relax"}, null },
                    { "stylize", "5.2", "--s", "100", "0", "1000", "Stylization strength setting", new string[] {"default", "fast", "relax"}, null },
                    { "stop", "5.2", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default", "fast", "relax"}, null },
                    { "imageWeight", "5.2", "--iw", null, null, null, "Image weight for image prompting", new string[] {"default", "fast", "relax"}, null },
                    { "chaos", "5.2", "--c", "0", "0", "100", "Chaos/randomness setting", new string[] {"default", "fast", "relax"}, null },
                    { "weird", "5.2", "--weird", "0", "0", "3000", "Weirdness level setting", new string[] {"default", "fast", "relax"}, null },
                    { "tile", "5.2", "--tile", null, null, null, "Creates seamless pattern tiles", new string[] {"default", "fast", "relax"}, null },
                    { "profile", "5.2", "--style", null, null, null, "Base style for the generation", new string[] {"default", "fast", "relax"}, new string[] {"raw", "cute", "expressive"} }
                });

            // Version 6 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_6",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes", "parameter_modes"],
                values: new object[,]
                {
                    { "aspectRatio", "6", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default", "fast", "relax", "turbo"}, new string[] {"1:1", "16:9", "9:16", "4:3", "3:4", "5:4", "4:5", "2:3", "3:2", "21:9", "9:21"} },
                    { "quality", "6", "--q", "1", "0.25", "5", "Generation quality setting", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "seed", "6", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "no", "6", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "stylize", "6", "--s", "100", "0", "1000", "Stylization strength setting", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "stop", "6", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "imageWeight", "6", "--iw", null, null, null, "Image weight for image prompting", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "chaos", "6", "--c", "0", "0", "100", "Chaos/randomness setting", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "weird", "6", "--weird", "0", "0", "3000", "Weirdness level setting", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "tile", "6", "--tile", null, null, null, "Creates seamless pattern tiles", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "profile", "6", "--style", null, null, null, "Base style for the generation", new string[] {"default", "fast", "relax", "turbo"}, new string[] {"raw", "cute", "expressive", "scenic"} },
                    { "repeat", "6", "--repeat", "1", "1", "40", "Number of images to generate", new string[] {"default", "fast", "relax", "turbo"}, null }
                });

            // Version 6.1 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_6_1",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes", "parameter_modes"],
                values: new object[,]
                {
                    { "aspectRatio", "6.1", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default", "fast", "relax", "turbo"}, new string[] {"1:1", "16:9", "9:16", "4:3", "3:4", "5:4", "4:5", "2:3", "3:2", "21:9", "9:21"} },
                    { "quality", "6.1", "--q", "1", "0.25", "5", "Generation quality setting", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "seed", "6.1", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "no", "6.1", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "stylize", "6.1", "--s", "100", "0", "1000", "Stylization strength setting", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "stop", "6.1", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "imageWeight", "6.1", "--iw", null, null, null, "Image weight for image prompting", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "chaos", "6.1", "--c", "0", "0", "100", "Chaos/randomness setting", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "weird", "6.1", "--weird", "0", "0", "3000", "Weirdness level setting", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "tile", "6.1", "--tile", null, null, null, "Creates seamless pattern tiles", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "profile", "6.1", "--style", null, null, null, "Base style for the generation", new string[] {"default", "fast", "relax", "turbo"}, new string[] {"raw", "cute", "expressive", "scenic", "photo"} },
                    { "repeat", "6.1", "--repeat", "1", "1", "40", "Number of images to generate", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "characterReference", "6.1", "--cref", null, null, null, "Content reference image", new string[] {"default", "fast", "relax", "turbo"}, null },
                    { "styleReference", "6.1", "--sref", null, null, null, "Style reference image", new string[] {"default", "fast", "relax", "turbo"}, null }
                });

            // Version 7 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_7",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes", "parameter_modes"],
                values: new object[,]
                {
                    { "aspectRatio", "7", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default", "fast", "relax", "turbo", "raw"}, new string[] {"1:1", "16:9", "9:16", "4:3", "3:4", "5:4", "4:5", "2:3", "3:2", "21:9", "9:21", "custom"} },
                    { "quality", "7", "--q", "1", "0.25", "5", "Generation quality setting", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "seed", "7", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "no", "7", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "stylize", "7", "--s", "100", "0", "1000", "Stylization strength setting", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "stop", "7", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "imageWeight", "7", "--iw", null, null, null, "Image weight for image prompting", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "chaos", "7", "--c", "0", "0", "100", "Chaos/randomness setting", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "weird", "7", "--weird", "0", "0", "3000", "Weirdness level setting", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "tile", "7", "--tile", null, null, null, "Creates seamless pattern tiles", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "profile", "7", "--style", null, null, null, "Base style for the generation", new string[] {"default", "fast", "relax", "turbo", "raw"}, new string[] {"raw", "cute", "expressive", "scenic", "photo", "illustration", "cinematic", "abstract"} },
                    { "repeat", "7", "--repeat", "1", "1", "40", "Number of images to generate", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "characterReference", "7", "--cref", null, null, null, "Content reference image", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "styleReference", "7", "--sref", null, null, null, "Style reference image", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "blend", "7", "--blend", null, null, null, "Blend multiple images together", new string[] {"default", "fast", "relax", "turbo", "raw"}, null },
                    { "raw", "7", "--raw", null, null, null, "Use raw mode for minimal styling", new string[] {"default", "fast", "relax", "turbo", "raw"}, null }
                });

            // Version niji_4 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_niji_4",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes", "parameter_modes"],
                values: new object[,]
                {
                    { "aspectRatio", "niji 4", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default"}, new string[] {"1:1", "16:9", "9:16", "4:3", "3:4", "5:4", "4:5"} },
                    { "quality", "niji 4", "--q", "1", "0.25", "2", "Generation quality setting", new string[] {"default"}, null },
                    { "seed", "niji 4", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default"}, null },
                    { "no", "niji 4", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default"}, null },
                    { "stylize", "niji 4", "--s", "100", "0", "750", "Stylization strength setting", new string[] {"default"}, null },
                    { "stop", "niji 4", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default"}, null },
                    { "niji", "niji 4", "--niji", null, null, null, "Niji-specific anime style modifiers", new string[] {"default"}, null }
                });

            // Version niji_5 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_niji_5",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes", "parameter_modes"],
                values: new object[,]
                {
                    { "aspectRatio", "niji 5", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default", "cute", "scenic", "expressive"}, new string[] {"1:1", "16:9", "9:16", "4:3", "3:4", "5:4", "4:5", "2:3", "3:2"} },
                    { "quality", "niji 5", "--q", "1", "0.25", "2", "Generation quality setting", new string[] {"default", "cute", "scenic", "expressive"}, null },
                    { "seed", "niji 5", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default", "cute", "scenic", "expressive"}, null },
                    { "no", "niji 5", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default", "cute", "scenic", "expressive"}, null },
                    { "stylize", "niji 5", "--s", "100", "0", "1000", "Stylization strength setting", new string[] {"default", "cute", "scenic", "expressive"}, null },
                    { "stop", "niji 5", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default", "cute", "scenic", "expressive"}, null },
                    { "profile", "niji 5", "--style", null, null, null, "Art style for the anime generation", new string[] {"default", "cute", "scenic", "expressive"}, new string[] {"cute", "scenic", "expressive"} },
                    { "niji", "niji 5", "--niji", "5", null, null, "Niji-specific anime style modifiers", new string[] {"default", "cute", "scenic", "expressive"}, null }
                });

            // Version niji_6 seeding
            migrationBuilder.InsertData(
                schema: "public",
                table: "version_niji_6",
                columns: ["PropertyName", "version", "parameter", "default_value", "min_value", "max_value", "description", "modes", "parameter_modes"],
                values: new object[,]
                {
                    { "aspectRatio", "niji 6", "--ar", "1:1", null, null, "Image aspect ratio", new string[] {"default", "cute", "scenic", "expressive", "original"}, new string[] {"1:1", "16:9", "9:16", "4:3", "3:4", "5:4", "4:5", "2:3", "3:2", "21:9", "9:21"} },
                    { "quality", "niji 6", "--q", "1", "0.25", "5", "Generation quality setting", new string[] {"default", "cute", "scenic", "expressive", "original"}, null },
                    { "seed", "niji 6", "--seed", null, null, null, "Seed number for reproducible results", new string[] {"default", "cute", "scenic", "expressive", "original"}, null },
                    { "no", "niji 6", "--no", null, null, null, "Negative prompting to exclude concepts", new string[] {"default", "cute", "scenic", "expressive", "original"}, null },
                    { "stylize", "niji 6", "--s", "100", "0", "1000", "Stylization strength setting", new string[] {"default", "cute", "scenic", "expressive", "original"}, null },
                    { "stop", "niji 6", "--stop", "100", "10", "100", "Stop generation at percentage", new string[] {"default", "cute", "scenic", "expressive", "original"}, null },
                    { "profile", "niji 6", "--style", null, null, null, "Art style for the anime generation", new string[] {"default", "cute", "scenic", "expressive", "original"}, new string[] {"cute", "scenic", "expressive", "original"} },
                    { "niji", "niji 6", "--niji", "6", null, null, "Niji-specific anime style modifiers", new string[] {"default", "cute", "scenic", "expressive", "original"}, null },
                    { "characterReference", "niji 6", "--cref", null, null, null, "Content reference image", new string[] {"default", "cute", "scenic", "expressive", "original"}, null },
                    { "styleReference", "niji 6", "--sref", null, null, null, "Style reference image", new string[] {"default", "cute", "scenic", "expressive", "original"}, null }
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
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stylize", "stop", "profile", "niji", "characterReference", "styleReference" });

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
                keyValues: new string[] { "aspectRatio", "quality", "seed", "no", "stop" });

            // Remove seeded data from version_master
            migrationBuilder.DeleteData(
                schema: "public",
                table: "version_master",
                keyColumn: "version",
                keyValues: new string[] { "1", "2", "3", "4", "5", "5.1", "5.2", "6", "6.1", "7", "niji 4", "niji 5", "niji 6" });
        }
    }
}
