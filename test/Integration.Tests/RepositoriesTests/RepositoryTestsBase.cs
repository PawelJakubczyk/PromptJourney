using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Persistence.Repositories;
using Persistence.Repositories.Utilities;
using Utilities.Results;

namespace Integration.Tests.RepositoriesTests;

public abstract class RepositoryTestsBase : BaseTransactionIntegrationTest
{
    // Repository Fields
    protected readonly VersionsRepository VersionsRepository;
    protected readonly StylesRepository StylesRepository;
    protected readonly ExampleLinkRepository ExampleLinkRepository;
    protected readonly PromptHistoryRepository PromptHistoryRepository;
    protected readonly PropertiesRepository PropertiesRepository;
    protected readonly CancellationToken CancellationToken = CancellationToken.None;
    protected readonly DefaultHybridCache Cache;

    // Constructor
    protected RepositoryTestsBase(MidjourneyDbFixture fixture) : base(fixture)
    {
        VersionsRepository = new VersionsRepository(DbContext, Cache);
        StylesRepository = new StylesRepository(DbContext, Cache);
        ExampleLinkRepository = new ExampleLinkRepository(DbContext, VersionsRepository, StylesRepository);
        PromptHistoryRepository = new PromptHistoryRepository(DbContext);
        PropertiesRepository = new PropertiesRepository(DbContext, Cache);
    }

    // Common Test Data Constants
    protected const string DefaultTestVersion1 = "1.0";
    protected const string DefaultTestVersion2 = "2.0";
    protected const string DefaultTestVersion3 = "6.0";

    protected const string DefaultTestStyleName1 = "TestStyle1";
    protected const string DefaultTestStyleName2 = "TestStyle2";
    protected const string DefaultTestStyleName3 = "TestStyle3";

    protected const string DefaultTestStyleType = "Custom";

    protected const string DefaultTestLink1 = "https://example.com/test1.jpg";
    protected const string DefaultTestLink2 = "https://example.com/test2.jpg";
    protected const string DefaultTestLink3 = "https://example.com/test3.jpg";

    protected const string DefaultTestPrompt1 = "A beautiful landscape with mountains";
    protected const string DefaultTestPrompt2 = "Modern city skyline at sunset";
    protected const string DefaultTestPrompt3 = "Abstract art with vibrant colors";

    protected const string DefaultTestPropertyName1 = "aspect";
    protected const string DefaultTestPropertyName2 = "quality";
    protected const string DefaultTestPropertyName3 = "stylize";

    protected const string DefaultTestParam1 = "--ar";
    protected const string DefaultTestParam2 = "--q";
    protected const string DefaultTestParam3 = "--s";

    // Version Helper Methods
    protected async Task<MidjourneyVersion> CreateAndSaveTestVersionAsync(string versionValue)
    {
        var version = ModelVersion.Create(versionValue).Value;
        var parameter = Param.Create($"--v {versionValue}").Value;
        var description = Description.Create($"Test version {versionValue}").Value;
        var releaseDateResult = ReleaseDate.Create(DateTime.UtcNow.ToString()).Value;

        var versionEntity = MidjourneyVersion.Create
        (
            Result.Ok(version),
            Result.Ok(parameter),
            Result.Ok(releaseDateResult),
            Result.Ok<Description?>(description)
        ).Value;

        var result = await VersionsRepository.AddVersionAsync(versionEntity, CancellationToken);

        AssertSuccessResult(result);
        return result.Value;
    }

    protected async Task<List<MidjourneyVersion>> CreateAndSaveMultipleVersionsAsync(params string[] versionValues)
    {
        var versions = new List<MidjourneyVersion>();
        foreach (var versionValue in versionValues)
        {
            versions.Add(await CreateAndSaveTestVersionAsync(versionValue));
        }
        return versions;
    }

    // Style Helper Methods
    protected async Task<MidjourneyStyle> CreateAndSaveTestStyleAsync(string styleName, string styleType = DefaultTestStyleType)
    {
        var name = StyleName.Create(styleName).Value;
        var type = StyleType.Create(styleType).Value;
        var description = Description.Create($"Test style {styleName}").Value;
        var tags = TagsCollection.Create(
        [
             $"TestTagFor{styleName}",
             $"TestTagFor{styleName}2"
        ]);

        var style = MidjourneyStyle.Create(
            Result.Ok(name),
            Result.Ok(type),
            Result.Ok(description),
            tags);

        var result = await StylesRepository.AddStyleAsync(style.Value, CancellationToken);
        AssertSuccessResult(result);
        return result.Value;
    }

    protected async Task<List<MidjourneyStyle>> CreateAndSaveMultipleStylesAsync(params string[] styleNames)
    {
        var styles = new List<MidjourneyStyle>();
        foreach (var styleName in styleNames)
        {
            styles.Add(await CreateAndSaveTestStyleAsync(styleName));
        }
        return styles;
    }

    protected async Task<MidjourneyStyle> CreateAndSaveTestStyleWithTagsAsync(string styleName, params string[] tagNames)
    {
        var style = await CreateAndSaveTestStyleAsync(styleName);

        foreach (var tagName in tagNames)
        {
            var tag = Tag.Create(tagName).Value;
            await StylesRepository.AddTagToStyleAsync(style.StyleName, tag, CancellationToken);
        }

        return style;
    }

    // ExampleLink Helper Methods
    protected async Task<MidjourneyStyleExampleLink> CreateAndSaveTestExampleLinkAsync(
        string linkUrl,
        string styleName,
        string versionValue)
    {
        var linkIdResult = Result.Ok(LinkID.Create().Value);
        var linkResult = ExampleLink.Create(linkUrl);
        var styleNameResult = StyleName.Create(styleName);
        var versionResult = ModelVersion.Create(versionValue);

        var exampleLink = MidjourneyStyleExampleLink.Create(
            linkIdResult,
            linkResult,
            styleNameResult,
            versionResult
        ).Value;

        var result = await ExampleLinkRepository.AddExampleLinkAsync(exampleLink, CancellationToken);
        AssertSuccessResult(result);
        return result.Value;
    }

    protected async Task<List<MidjourneyStyleExampleLink>> CreateAndSaveMultipleExampleLinksAsync(
        IEnumerable<(string linkUrl, string styleName, string versionValue)> linkData)
    {
        var links = new List<MidjourneyStyleExampleLink>();
        foreach (var (linkUrl, styleName, versionValue) in linkData)
        {
            links.Add(await CreateAndSaveTestExampleLinkAsync(linkUrl, styleName, versionValue));
        }
        return links;
    }

    // PromptHistory Helper Methods
    protected async Task<MidjourneyPromptHistory> CreateAndSaveTestPromptHistoryAsync(
        string promptText,
        MidjourneyVersion version,
        List<MidjourneyStyle> styles)
    {
        var promptResult = Prompt.Create(promptText);
        var versionResult = ModelVersion.Create(version.Version.Value); // lub odpowiednia wartość
        var createdOnResult = CreatedOn.Create(DateTime.UtcNow.ToString("O"));

        var promptHistory = MidjourneyPromptHistory.Create(
            Result.Ok(HistoryID.Create().Value),
            promptResult,
            versionResult,
            createdOnResult
        ).Value;

        // Add styles if provided
        foreach (var style in styles)
        {
            if (promptHistory.MidjourneyStyles is List<MidjourneyStyle> styleList)
            {
                styleList.Add(style);
            }
        }

        var result = await PromptHistoryRepository.AddPromptToHistoryAsync(promptHistory, CancellationToken);
        AssertSuccessResult(result);
        return result.Value;
    }

    protected static Task<MidjourneyPromptHistory> CreateTestPromptHistoryAsync(
        string promptText,
        MidjourneyVersion version,
        List<MidjourneyStyle> styles)
    {
        var historyIdResult = Result.Ok(HistoryID.Create().Value);
        var promptResult = Prompt.Create(promptText);
        var versionResult = ModelVersion.Create(version.Version.Value); // lub odpowiednia wartość
        var createdOnResult = CreatedOn.Create(DateTime.UtcNow.ToString("O"));

        var promptHistory = MidjourneyPromptHistory.Create(
            historyIdResult,
            promptResult,
            versionResult,
            createdOnResult
        ).Value;

        // Add styles if provided
        foreach (var style in styles)
        {
            if (promptHistory.MidjourneyStyles is List<MidjourneyStyle> styleList)
            {
                styleList.Add(style);
            }
        }

            return Task.FromResult(promptHistory);
    }

    // Properties Helper Methods
    protected static async Task<MidjourneyProperty> CreateTestPropertyAsync(
        string version,
        string propertyName,
        List<string> parameters,
        string? defaultValue = null,
        string? minValue = null,
        string? maxValue = null,
        string? description = null)
    {
        var propertyNameVo = PropertyName.Create(propertyName).Value;
        var versionVo = ModelVersion.Create(version).Value;

        var parametersResult = ParamsCollection.Create(parameters);

        var defaultValueResult = defaultValue != null ? DefaultValue.Create(defaultValue) : Result.Ok<DefaultValue?>(null);
        var minValueResult = minValue != null ? MinValue.Create(minValue) : Result.Ok<MinValue?>(null);
        var maxValueResult = maxValue != null ? MaxValue.Create(maxValue) : Result.Ok<MaxValue?>(null);
        var descriptionResult = description != null ? Description.Create(description) : Result.Ok<Description?>(null);

        var property = MidjourneyProperty.Create(
            Result.Ok(propertyNameVo),
            Result.Ok(versionVo),
            parametersResult,
            defaultValueResult,
            minValueResult,
            maxValueResult,
            descriptionResult).Value;

        return property;
    }

    protected async Task<MidjourneyProperty> CreateAndSaveTestPropertyAsync(
        string version,
        string propertyName,
        List<string> parameters,
        string? defaultValue = null,
        string? minValue = null,
        string? maxValue = null,
        string? description = null)
    {
        var property = await CreateTestPropertyAsync(version, propertyName, parameters, defaultValue, minValue, maxValue, description);
        var result = await PropertiesRepository.AddPropertyAsync(property, CancellationToken);

        AssertSuccessResult(result);
        return result.Value;
    }

    // Test Setup Helper Methods
    protected async Task<(MidjourneyVersion version, MidjourneyStyle style)> CreateBasicTestDataAsync(
        string versionValue = DefaultTestVersion1,
        string styleName = DefaultTestStyleName1)
    {
        var version = await CreateAndSaveTestVersionAsync(versionValue);
        var style = await CreateAndSaveTestStyleAsync(styleName);
        return (version, style);
    }

    protected async Task<(List<MidjourneyVersion> versions, List<MidjourneyStyle> styles)> CreateMultipleTestDataAsync(
        string[]? versionValues = null,
        string[]? styleNames = null)
    {
        versionValues ??= [DefaultTestVersion1, DefaultTestVersion2, DefaultTestVersion3];
        styleNames ??= [DefaultTestStyleName1, DefaultTestStyleName2, DefaultTestStyleName3];

        var versions = await CreateAndSaveMultipleVersionsAsync(versionValues);
        var styles = await CreateAndSaveMultipleStylesAsync(styleNames);

        return (versions, styles);
    }

    // Assertion Helper Methods
    protected static void AssertSuccessResult<T>(Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    protected static void AssertFailureResult<T>(Result<T> result, string? expectedErrorMessage = null)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        if (!string.IsNullOrEmpty(expectedErrorMessage))
        {
            result.Errors.Should().Contain(e => e.Message.Contains(expectedErrorMessage));
        }
    }
}
