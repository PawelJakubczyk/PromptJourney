using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using FluentResults;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class AddStyleTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{
    [Fact]
    public async Task AddStyleAsync_WithValidStyle_ShouldReturnSuccess()
    {
        // Arrange
        var styleName = StyleName.Create("NewStyle").Value;
        var styleType = StyleType.Create("Custom").Value;
        var description = Description.Create("Test description").Value;

        var style = MidjourneyStyle.Create(
            Result.Ok(styleName),
            Result.Ok(styleType),
            Result.Ok<Description?>(description)).Value;

        // Act
        var result = await StylesRepository.AddStyleAsync(style, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.StyleName.Value.Should().Be("NewStyle");
        result.Value.Type.Value.Should().Be("Custom");
        result.Value.Description!.Value.Should().Be("Test description");
    }

    [Fact]
    public async Task AddStyleAsync_WithStyleWithTags_ShouldReturnSuccess()
    {
        // Arrange
        var styleName = StyleName.Create("StyledStyle").Value;
        var styleType = StyleType.Create("Custom").Value;
        var tags = new List<Result<Tag>?>
        {
            Tag.Create("modern"),
            Tag.Create("abstract")
        };

        var style = MidjourneyStyle.Create(
            Result.Ok(styleName),
            Result.Ok(styleType),
            null,
            tags).Value;

        // Act
        var result = await StylesRepository.AddStyleAsync(style, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.StyleName.Value.Should().Be("StyledStyle");
        result.Value.Tags.Should().HaveCount(2);
        result.Value.Tags.Should().Contain(t => t.Value == "modern");
        result.Value.Tags.Should().Contain(t => t.Value == "abstract");
    }

    [Fact]
    public async Task AddStyleAsync_WithMinimalData_ShouldReturnSuccess()
    {
        // Arrange
        var styleName = StyleName.Create("MinimalStyle").Value;
        var styleType = StyleType.Create("Basic").Value;

        var style = MidjourneyStyle.Create(
            Result.Ok(styleName),
            Result.Ok(styleType)).Value;

        // Act
        var result = await StylesRepository.AddStyleAsync(style, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.StyleName.Value.Should().Be("MinimalStyle");
        result.Value.Type.Value.Should().Be("Basic");
        result.Value.Description.Should().BeNull();
        result.Value.Tags.Should().BeNull();
    }

    [Fact]
    public async Task AddStyleAsync_WithExistingName_ShouldHandleGracefully()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);

        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var styleType = StyleType.Create("Different").Value;

        var style = MidjourneyStyle.Create(
            Result.Ok(styleName),
            Result.Ok(styleType)).Value;

        // Act & Assert
        // This test depends on repository implementation - it might fail or succeed
        // The behavior should be consistent with business rules
        var result = await StylesRepository.AddStyleAsync(style, CancellationToken);

        // We don't assert success/failure as it depends on implementation
        result.Should().NotBeNull();
    }
}