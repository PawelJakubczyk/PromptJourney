using Application.Abstractions.IRepository;
using Application.Features.Properties;
using Domain.ValueObjects;
using FluentResults;
using System.Reflection;

namespace Application;

public class Validate
{


    public static class Link
    {    
        public static async Task<Result<string>> ShouldExists(ExampleLink link, IExampleLinksRepository repository)
        {
            var exampleLinkExistsResult = await repository.CheckExampleLinkExistsAsync(link);
            if (exampleLinkExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if example link '{link}' exists");
            }
            if (!exampleLinkExistsResult.Value)
            {
                return Result.Fail<string>($"Example link '{link}' does not exist'");
            }
            return Result.Ok<string>($"Example link '{link}' does exist. You may proceed with the operation.");
        }

        public static async Task<Result<string>> ShouldNotExists(ExampleLink link, IExampleLinksRepository repository)
        {
            var exampleLinkExistsResult = await repository.CheckExampleLinkExistsAsync(link);
            if (exampleLinkExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if example link '{link}' exists");
            }
            if (exampleLinkExistsResult.Value)
            {
                return Result.Fail<string>($"Example link '{link}' already exists.");
            }
            return Result.Ok<string>($"Example link '{link}' does not exist. You may proceed with the operation.");
        }
    }

    public static class Links
    {
        public static async Task<Result<string>> ShouldHaveAtLastOneElement(IExampleLinksRepository repository)
        {
            var exampleLinkExistsResult = await repository.CheckExampleLinksAreNotEmpty();
            if (exampleLinkExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if example links have any element");
            }
            if (exampleLinkExistsResult.Value)
            {
                return Result.Fail<string>($"Example links have no elemements");
            }
            return Result.Ok<string>($"Example links have at last one element. You may proceed with the operation.'");
        }
    }

    public static class History
    {
        public static Task<Result<string>> LimitMustBeGreaterThanZero(int limit)
        {
            if (limit < 0)
            {
                return Task.FromResult(Result.Fail<string>("The limit must be greater than zero."));
            }
            return Task.FromResult(Result.Ok<string>("The limit is valid – it is greater than zero. You may proceed with the operation."));
        }

        public static Task<Result<string>> CountMustNotExceedHistoricalRecords(int count, IPromptHistoryRepository repository)
        {
            if (count > repository.CalculateHistoricalRecordCountAsync().Result.Value)
            {
                return Task.FromResult(Result.Fail<string>($"The count ({count}) exceeds the number of historical records available."));
            }
            return Task.FromResult(Result.Ok<string>($"The count ({count}) is within the available historical records. You may proceed with the operation."));
        }
    }

    public static class Date
    {
        public static Task<Result<string>> ShouldNotBeInFuture(DateTime inputDate)
        {
            if (inputDate > DateTime.UtcNow)
            {
                return Task.FromResult(Result.Fail<string>("The provided date cannot be in the future."));
            }

            return Task.FromResult(Result.Ok<string>("The date is valid – it is not in the future. You may proceed with the operation."));
        }

        public static class Range
        {
            public static Task<Result<string>> ShouldBeChronological(DateTime startDate, DateTime endDate)
            {
                if (startDate >= endDate)
                {
                    return Task.FromResult(Result.Fail<string>(
                        $"Invalid date range: the start date ({startDate:yyyy-MM-dd}) must be earlier than the end date ({endDate:yyyy-MM-dd})."));
                }

                return Task.FromResult(Result.Ok<string>(
                    $"Valid date range: from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}. You may proceed with the operation."));
            }
        }
    }

    public static class Property
    {
        public static class Name
        {

        }

        public static class Parameters
        {
            public static class Input
            {

            }
        }

        public static async Task<Result<string>> ShouldNotExists(ModelVersion version, PropertyName PropertyName, IPropertiesRepository repository)
        {
            var parameterExistsResult = await repository.CheckParameterExistsInVersionAsync(version, PropertyName);
            if (parameterExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if parameter exists for version '{version}'");
            }

            if (parameterExistsResult.Value)
            {
                return Result.Fail<string>($"Parameter '{PropertyName}' already exists for version '{version}'");
            }
            return Result.Ok<string>($"Parameter '{PropertyName}' does not exist for version '{version}', You may proceed with the operation.");
        }

        public static async Task<Result<string>> ShouldExists(ModelVersion version, PropertyName PropertyName, IPropertiesRepository repository)
        {
            var parameterExistsResult = await repository.CheckParameterExistsInVersionAsync(version, PropertyName);
            if (parameterExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if parameter exists for version '{version}'");
            }

            if (!parameterExistsResult.Value)
            {
               return Result.Fail<string>($"Parameter '{PropertyName}' does not exist for version '{version}'");
            }
            return Result.Ok<string>($"Parameter '{PropertyName}' does exists for version '{version}', You may proceed with the operation.");
        }

        public static async Task<Result<string>> ShouldMatchingPropertyName(string input)
        {
            var propertyNames = typeof(PropertyDetails)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name);
            if (propertyNames is null || !propertyNames.Any())
            {
                return Result.Fail<string>("No public instance properties found in PropertyDetails.");
            }
            return Result.Ok<string>($"The input '{input}' is valid – it matches a property name in PropertyDetails. You may proceed with the operation.");
        }
    }

    public static class Style
    {
        public static async Task<Result<string>> ShouldExists(StyleName styleName, IStyleRepository repository)
        {
            var styleExistsResult = await repository.CheckStyleExistsAsync(styleName);
            if (styleExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if style '{styleName}' exists");
            }
            if (!styleExistsResult.Value)
            {
                return Result.Fail<string>($"Style '{styleName}' does not exist");
            }
            return Result.Ok<string>($"Style '{styleName}' does exists, You may proceed with the operation.");
        }
        public static async Task<Result<string>> ShouldNotExists(StyleName styleName, IStyleRepository repository)
        {
            var styleExistsResult = await repository.CheckStyleExistsAsync(styleName);
            if (styleExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if style '{styleName}' exists");
            }
            if (styleExistsResult.Value)
            {
                return Result.Fail<string>($"Style '{styleName}' already exists");
            }
            return Result.Ok<string>($"Style '{styleName}' does not exist, You may proceed with the operation.");
        }
    }

    public static class Tags
    {
        public static async Task<Result<string>> ShouldExists(StyleName styleName, Tag tag, IStyleRepository repository)
        {
            var tagExistsResult = await repository.CheckTagExistsInStyleAsync(styleName, tag);
            if (tagExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if tag: '{tag}' for style:'{styleName}' exists");
            }
            if (!tagExistsResult.Value)
            {
                return Result.Fail<string>($"Tag: '{tag}' for style:'{styleName}' does not exist");
            }
            return Result.Ok<string>($"Tag: '{tag}' for  Style: '{styleName}' does exist, You may proceed with the operation.");
        }
        public static async Task<Result<string>> ShouldNotExists(StyleName styleName, Tag tag, IStyleRepository repository)
        {
            var tagExistsResult = await repository.CheckTagExistsInStyleAsync(styleName, tag);
            if (tagExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if tag: '{tag}' for  style: '{styleName}' exists");
            }
            if (tagExistsResult.Value)
            {
                return Result.Fail<string>($"Tag: '{tag}' for  Style: '{styleName}' already exists");
            }
            return Result.Ok<string>($"Tag: '{tag}' for  Style: '{styleName}' does not exist, You may proceed with the operation.");
        }
    }

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