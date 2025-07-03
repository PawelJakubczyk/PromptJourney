using Application.Abstractions;
using Application.Features.Versions.Commands.AddParameterToVersion;
using FluentResults;

namespace Application.Features.Versions;

public class Validate
{
    public static class Version
    {
        public static async Task ShouldExists( string version, IVersionRepository repository)
        {
            var versionExistsResult = await repository.CheckVersionExistsInVersionMasterAsync(version);

            if (versionExistsResult.IsFailed)
            {
                Result.Fail<ParameterDetails>($"Failed to check if version exists");
            }

            if (!versionExistsResult.Value)
            {
                Result.Fail<ParameterDetails>($"Version '{version}' not found");
            }
        }
    }

    public static class Parameter
    {
        public static async Task ShouldNotExists(string version, string parameterName, IVersionRepository repository)
        {
            var parameterExistsResult = await repository.CheckParameterExistsInVersionAsync(version, parameterName);
            if (parameterExistsResult.IsFailed)
            {
                Result.Fail<ParameterDetails>($"Failed to check if parameter exists for version '{version}'");
            }

            if (parameterExistsResult.Value)
            {
                Result.Fail<ParameterDetails>($"Parameter '{parameterName}' already exists for version '{version}'");
            }
        }

        public static async Task ShouldExists(string version, string parameterName, IVersionRepository repository)
        {
            var parameterExistsResult = await repository.CheckParameterExistsInVersionAsync(version, parameterName);
            if (parameterExistsResult.IsFailed)
            {
                Result.Fail<ParameterDetails>($"Failed to check if parameter exists for version '{version}'");
            }

            if (!parameterExistsResult.Value)
            {
                Result.Fail<ParameterDetails>($"Parameter '{parameterName}' does not exist for version '{version}'");
            }
        }
    }
}