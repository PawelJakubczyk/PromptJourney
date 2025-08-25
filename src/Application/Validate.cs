using Application.Abstractions.IRepository;
using Application.Features.Properties;
using FluentResults;
using System.Reflection;

namespace Application;

public class Validate
{
    public static class Input
    {
        public static Task<Result<string>> MustNotBeNullOrEmpty(string input, string parameterName)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Task.FromResult(Result.Fail<string>($"The input '{parameterName}' cannot be null or empty."));
            }
            return Task.FromResult(Result.Ok<string>($"The input '{parameterName}' is valid – it is not null or empty. You may proceed with the operation."));
        }

        public static Task<Result<string>> MustHaveMaximumLength(string input, int maxLength, string parameterName)
        {
            if (input.Length > maxLength)
            {
                return Task.FromResult(Result.Fail<string>($"The input '{parameterName}' is invalid - it does exceed the maximum length of {maxLength} characters."));
            }
            return Task.FromResult(Result.Ok<string>($"The input '{parameterName}' is valid – it does not exceed the maximum length of {maxLength} characters. You may proceed with the operation."));
        }
    }

    public static class Entity
    {
        public static Task<Result<string>> MustNotBeNull(object entity, string entityName)
        {
            if (entity is null)
            {
                return Task.FromResult(Result.Fail<string>($"The entity '{entityName}' cannot be null."));
            }
            return Task.FromResult(Result.Ok<string>($"The entity '{entityName}' is valid – it is not null. You may proceed with the operation."));
        }


    }

    public static class Link
    {
        public static class Input
        {
            public static async Task<Result<string>> MustNotBeNullOrEmpty(string input)
            {
                return await Validate.Input.MustNotBeNullOrEmpty(input, nameof(Link));
            }

            public static async Task<Result<string>> MustHaveMaximumLength(string input)
            {
                return await Validate.Input.MustHaveMaximumLength(input, 100, nameof(Link));
            }
        }
        
        public static async Task<Result<string>> ShouldExists(string link, IExampleLinksRepository repository)
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

        public static async Task<Result<string>> ShouldNotExists(string link, IExampleLinksRepository repository)
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
        public static async Task<Result<string>> ShouldHaveAtLastOneElementWithStyle(string style, IExampleLinksRepository repository)
        {
            var exampleLinkWithStyleExistsResult = await repository.CheckExampleLinkWithStyleExistsAsync(style);
            if (exampleLinkWithStyleExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if example link '{style}' exists");
            }
            if (!exampleLinkWithStyleExistsResult.Value)
            {
                return Result.Fail<string>($"Example link with '{style}' does not exist'");
            }

            return Result.Ok<string>($"Example link with '{style}' exist. You may proceed with the operation.'");
        }

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
        public static class Input
        {
            public static async Task<Result<string>> MustNotBeNullOrEmpty(string input)
            {
                return await Validate.Input.MustNotBeNullOrEmpty(input, nameof(Link));
            }

            public static async Task<Result<string>> MustHaveMaximumLength(string input)
            {
                return await Validate.Input.MustHaveMaximumLength(input, 1000, nameof(Link));
            }

            public class Keyword
            {
                public static async Task<Result<string>> MustNotBeNullOrEmpty(string input)
                {
                    return await Validate.Input.MustNotBeNullOrEmpty(input, nameof(Keyword));
                }
            }
        }

        public static Task<Result<string>> LimitMustBeGreaterThanZero(int limit)
        {
            if (limit < 0)
            {
                return Task.FromResult(Result.Fail<string>("The limit must be greater than zero."));
            }
            return Task.FromResult(Result.Ok<string>("The limit is valid – it is greater than zero. You may proceed with the operation."));
        }

        public static Task<Result<string>> CountMustNotExceedHistoricalRecords(int cout, IPromptHistoryRepository repository)
        {
            if (cout > repository.CalculateHistoricalRecordCountAsync().Result.Value)
            {
                return Task.FromResult(Result.Fail<string>($"The count ({cout}) exceeds the number of historical records available."));
            }
            return Task.FromResult(Result.Ok<string>($"The count ({cout}) is within the available historical records. You may proceed with the operation."));
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
            public static class Input
            {
                public static async Task<Result<string>> MustNotBeNullOrEmpty(string input)
                {
                    return await Validate.Input.MustNotBeNullOrEmpty(input, nameof(Link));
                }
                public static async Task<Result<string>> MustHaveMaximumLength(string input)
                {
                    return await Validate.Input.MustHaveMaximumLength(input, 100, nameof(Link));
                }
            }
        }

        public static class Parameters
        {
            public static class Input
            {
                public static async Task<Result<string>> MustNotBeNull(string[] input)
                {
                    if (input is null)
                    {
                        return Result.Fail<string>($"The input 'Parameters' cannot be null.");
                    }
                    return Result.Ok<string>($"The input 'Parameters' is valid – it is not null. You may proceed with the operation.");
                }
                public static async Task<Result<string>> MustHaveAtLeastOneElement(string[] input)
                {
                    if (input.Length < 1)
                    {
                        return Result.Fail<string>($"The input 'Parameters' must have at least one element.");
                    }
                    return Result.Ok<string>($"The input 'Parameters' is valid – it has at least one element. You may proceed with the operation.");
                }
                public static async Task<Result<string>> MustNotHaveMoreThanXElements(string[] input, int x)
                {
                    if (input.Length > x)
                    {
                        return Result.Fail<string>($"The input 'Parameters' must not have more than {x} elements.");
                    }
                    return Result.Ok<string>($"The input 'Parameters' is valid – it does not have more than {x} elements. You may proceed with the operation.");
                }
                public static async Task<Result<string>> MustNotHaveElementsThatAreNullOrEmpty(string[] input)
                {
                    if (input.Any(string.IsNullOrEmpty))
                    {
                        return Result.Fail<string>($"The input 'Parameters' must not contain elements that are null or empty.");
                    }
                    return Result.Ok<string>($"The input 'Parameters' is valid – it does not contain elements that are null or empty. You may proceed with the operation.");
                }
            }
        }

        public static async Task<Result<string>> ShouldNotExists(string version, string parameterName, IPropertiesRopository repository)
        {
            var parameterExistsResult = await repository.CheckParameterExistsInVersionAsync(version, parameterName);
            if (parameterExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if parameter exists for version '{version}'");
            }

            if (parameterExistsResult.Value)
            {
                return Result.Fail<string>($"Parameter '{parameterName}' already exists for version '{version}'");
            }
            return Result.Ok<string>($"Parameter '{parameterName}' does not exist for version '{version}', You may proceed with the operation.");
        }

        public static async Task<Result<string>> ShouldExists(string version, string parameterName, IPropertiesRopository repository)
        {
            var parameterExistsResult = await repository.CheckParameterExistsInVersionAsync(version, parameterName);
            if (parameterExistsResult.IsFailed)
            {
                return Result.Fail<string>($"Failed to check if parameter exists for version '{version}'");
            }

            if (!parameterExistsResult.Value)
            {
               return Result.Fail<string>($"Parameter '{parameterName}' does not exist for version '{version}'");
            }
            return Result.Ok<string>($"Parameter '{parameterName}' does exists for version '{version}', You may proceed with the operation.");
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
        public static class Input
        {
            public static async Task<Result<string>> MustNotBeNullOrEmpty(string input)
            {
                return await Validate.Input.MustNotBeNullOrEmpty(input, nameof(Link));
            }

            public static async Task<Result<string>> MustHaveMaximumLenght(string input)
            {
                return await Validate.Input.MustHaveMaximumLength(input, 100, nameof(Link));
            }
        }

        public static async Task<Result<string>> ShouldExists(string styleName, IStyleRepository repository)
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
        public static async Task<Result<string>> ShouldNotExists(string styleName, IStyleRepository repository)
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

    public static class Tag
    {
        public static async Task<Result<string>> ShouldExists(string styleName, string tag, IStyleRepository repository)
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
        public static async Task<Result<string>> ShouldNotExists(string styleName, string tag, IStyleRepository repository)
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
        public static class Input
        {
            public static async Task<Result<string>> MustNotBeNullOrEmpty(string input)
            {
                return await Validate.Input.MustNotBeNullOrEmpty(input, nameof(Version));
            }

            public static async Task<Result<string>> MustHaveMaximumLength(string input)
            {
                return await Validate.Input.MustHaveMaximumLength(input, 10, nameof(Version));
            }
        }

        public static class Parameter
        {
            public static class Input
            {
                public static async Task<Result<string>> MustNotBeNullOrEmpty(string input)
                {
                    return await Validate.Input.MustNotBeNullOrEmpty(input, nameof(Parameter));
                }
                public static async Task<Result<string>> MustHaveMaximumLength(string input)
                {
                    return await Validate.Input.MustHaveMaximumLength(input, 15, nameof(Parameter));
                }
            }
        }

        public static async Task<Result<string>> ShouldExists(string version, IVersionRepository repository)
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