using Application.Abstractions.IRepository;
using Application.Features.Properties;
using Domain.ValueObjects;
using FluentResults;
using System.Reflection;

namespace Application;

public class Validate
{




    public static class Version
    {
        public static async Task<Result<string>> ShouldExists(ModelVersion version, IVersionRepository repository)
        {
            var versionExistsResult = await repository.CheckVersionExistsInVersionsAsync(version);

            if (versionExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if version exists");
            }

            if (!versionExistsResult.Value)
            {
                return Result.Fail<string>($"Version '{version}' not found");
            }
            return Result.Ok<string>($"Version: '{version}' does exist, You may proceed with the operation.");
        }

        public static async Task<Result<string>> ShouldHaveAnySuportedVersion(IVersionRepository repository)
        {
            var suportedVersionExistResult = await repository.CheckIfAnySupportedVersionExistsAsync();

            if (suportedVersionExistResult.Value)
            {
                return Result.Fail<string>("No supported versions found.");
            }
            return Result.Ok<string>($"Supported versions are available. You may proceed with the operation.");
        }

    }
}