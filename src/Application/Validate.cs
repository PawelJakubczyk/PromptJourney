using Application.Abstractions;
using Application.Features.Properties;
using FluentResults;

namespace Application;

public partial class Validate
{
    public static class Property
    {
        public static async Task ShouldNotExists(string version, string parameterName, IPropertiesRopository repository)
        {
            var parameterExistsResult = await repository.CheckParameterExistsInVersionAsync(version, parameterName);
            if (parameterExistsResult.IsFailed)
            {
                Result.Fail<PropertyDetails>($"Failed to check if parameter exists for version '{version}'");
            }

            if (parameterExistsResult.Value)
            {
                Result.Fail<PropertyDetails>($"Parameter '{parameterName}' already exists for version '{version}'");
            }
        }

        public static async Task ShouldExists(string version, string parameterName, IPropertiesRopository repository)
        {
            var parameterExistsResult = await repository.CheckParameterExistsInVersionAsync(version, parameterName);
            if (parameterExistsResult.IsFailed)
            {
                Result.Fail<PropertyDetails>($"Failed to check if parameter exists for version '{version}'");
            }

            if (!parameterExistsResult.Value)
            {
                Result.Fail<PropertyDetails>($"Parameter '{parameterName}' does not exist for version '{version}'");
            }
        }
    }

    public static class Version
    {
        public static async Task ShouldExists(string version, IVersionRepository repository)
        {
            var versionExistsResult = await repository.CheckVersionExistsInVersionMasterAsync(version);

            if (versionExistsResult.IsFailed)
            {
                Result.Fail<PropertyDetails>($"Failed to check if version exists");
            }

            if (!versionExistsResult.Value)
            {
                Result.Fail<PropertyDetails>($"Version '{version}' not found");
            }
        }
    }
}