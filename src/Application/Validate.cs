using Application.Abstractions.IRepository;
using Application.Features.Properties;
using FluentResults;
using static Application.Validate;

namespace Application;

public partial class Validate
{
    public static class History
    {
    }


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

    public static class Link
    {
        public static async Task ShouldExists(string link, IExampleLinksRepository repository)
        {
            var exampleLinkExistsResult = await repository.CheckExampleLinkExistsAsync(link);
            if (exampleLinkExistsResult.IsFailed)
            {
                Result.Fail<PropertyDetails>($"Failed to check if example link '{link}' exists");
            }
            if (!exampleLinkExistsResult.Value)
            {
                Result.Fail<PropertyDetails>($"Example link '{link}' does not exist'");
            }
        }

        public static async Task ShouldNotExists(string link, IExampleLinksRepository repository)
        {
            var exampleLinkExistsResult = await repository.CheckExampleLinkExistsAsync(link);
            if (exampleLinkExistsResult.IsFailed)
            {
                Result.Fail<PropertyDetails>($"Failed to check if example link '{link}' exists");
            }
            if (exampleLinkExistsResult.Value)
            {
                Result.Fail<PropertyDetails>($"Example link '{link}' already exists");
            }
        }
    }

    public static class Style
    {
        public static async Task ShouldExists(string styleName, IStyleRepository repository)
        {
            var styleExistsResult = await repository.CheckStyleExistsAsync(styleName);
            if (styleExistsResult.IsFailed)
            {
                Result.Fail<PropertyDetails>($"Failed to check if style '{styleName}' exists");
            }
            if (!styleExistsResult.Value)
            {
                Result.Fail<PropertyDetails>($"Style '{styleName}' does not exist");
            }
        }
        public static async Task ShouldNotExists(string styleName, IStyleRepository repository)
        {
            var styleExistsResult = await repository.CheckStyleExistsAsync(styleName);
            if (styleExistsResult.IsFailed)
            {
                Result.Fail<PropertyDetails>($"Failed to check if style '{styleName}' exists");
            }
            if (styleExistsResult.Value)
            {
                Result.Fail<PropertyDetails>($"Style '{styleName}' already exists");
            }
        }
    }

    public static class Tag
    {
        public static async Task ShouldExists(string styleName, string tag, IStyleRepository repository)
        {
            var tagExistsResult = await repository.CheckTagExistsAsync(styleName, tag);
            if (tagExistsResult.IsFailed)
            {
                Result.Fail<PropertyDetails>($"Failed to check if tag '{tag}' exists");
            }
            if (!tagExistsResult.Value)
            {
                Result.Fail<PropertyDetails>($"Tag '{tag}' does not exist");
            }
        }
        public static async Task ShouldNotExists(string styleName, string tag, IStyleRepository repository)
        {
            var tagExistsResult = await repository.CheckTagExistsAsync(styleName, tag);
            if (tagExistsResult.IsFailed)
            {
                Result.Fail<PropertyDetails>($"Failed to check if tag '{tag}' exists");
            }
            if (tagExistsResult.Value)
            {
                Result.Fail<PropertyDetails>($"Tag '{tag}' already exists");
            }
        }
    }

    public static class Version
    {
        public static async Task ShouldExists(string version, IVersionRepository repository)
        {
            var versionExistsResult = await repository.CheckVersionExistsInVersionsAsync(version);

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