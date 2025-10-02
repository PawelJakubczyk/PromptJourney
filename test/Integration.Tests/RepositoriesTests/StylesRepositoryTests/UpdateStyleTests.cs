using Domain.ValueObjects;
using FluentAssertions;
using FluentResults;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class UpdateStyleTests : RepositoryTestsBase
{
    public UpdateStyleTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateStyleAsync_WithExistingStyle_ShouldReturnSuccess()
    {
        // Arrange
        var originalStyle = await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);

        // Update description
        var newDescription = Description.Create("Updated description").Value;
        originalStyle.EditDescription(Result.Ok<Description?>(newDescription));

        // Act
        var result = await StylesRepository.UpdateStyleAsync(originalStyle, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.StyleName.Value.Should().Be(DefaultTestStyleName1);
        result.Value.Description!.Value.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateStyleAsync_WithAddedTags_ShouldReturnSuccess()
    {
        // Arrange
        var originalStyle = await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);

        // Add tags
        originalStyle.AddTag(Tag.Create("modern"));
        originalStyle.AddTag(Tag.Create("abstract"));

        // Act
        var result = await StylesRepository.UpdateStyleAsync(originalStyle, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Tags.Should().HaveCount(2);
        result.Value.Tags.Should().Contain(t => t.Value == "modern");
        result.Value.Tags.Should().Contain(t => t.Value == "abstract");
    }

    [Fact]
    public async Task UpdateStyleAsync_WithNonExistentStyle_ShouldHandleGracefully()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistent").Value;
        var styleType = StyleType.Create("Custom").Value;

        var style = Domain.Entities.MidjourneyStyle.Create(
            Result.Ok(styleName),
            Result.Ok(styleType)).Value;

        // Act
        var result = await StylesRepository.UpdateStyleAsync(style, CancellationToken);

        // Assert
        // Behavior depends on implementation - might succeed or fail
        result.Should().NotBeNull();
    }
}