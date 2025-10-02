using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.ExampleLinksRepositoryTests;

public class CheckExampleLinkWithStyleExistsTests : RepositoryTestsBase
{
    public CheckExampleLinkWithStyleExistsTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    // CheckExampleLinkWithStyleExistsAsync Tests
    [Fact]
    public async Task CheckExampleLinkWithStyleExistsAsync_WithExistingStyle_ShouldReturnTrue()
    {
        // Arrange
        await CreateBasicTestDataAsync();
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);

        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await ExampleLinkRepository.CheckExampleLinkWithStyleExistsAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckExampleLinkWithStyleExistsAsync_WithNonExistentStyle_ShouldReturnFalse()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await ExampleLinkRepository.CheckExampleLinkWithStyleExistsAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckExampleLinkWithStyleExistsAsync_WithExistingStyleButNoLinks_ShouldReturnFalse()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await ExampleLinkRepository.CheckExampleLinkWithStyleExistsAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }
}