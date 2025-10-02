using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.ExampleLinksRepositoryTests;

public class CheckAnyExampleLinksExistTests : RepositoryTestsBase
{
    public CheckAnyExampleLinksExistTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    // CheckAnyExampleLinksExistAsync Tests
    [Fact]
    public async Task CheckAnyExampleLinksExistAsync_WithExistingLinks_ShouldReturnTrue()
    {
        // Arrange
        await CreateBasicTestDataAsync();
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);

        // Act
        var result = await ExampleLinkRepository.CheckAnyExampleLinksExistAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckAnyExampleLinksExistAsync_WithNoLinks_ShouldReturnFalse()
    {
        // Act
        var result = await ExampleLinkRepository.CheckAnyExampleLinksExistAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckAnyExampleLinksExistAsync_WithMultipleLinks_ShouldReturnTrue()
    {
        // Arrange
        var (versions, styles) = await CreateMultipleTestDataAsync();

        var linkData = new[]
        {
            (DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1),
            (DefaultTestLink2, DefaultTestStyleName2, DefaultTestVersion2)
        };
        await CreateAndSaveMultipleExampleLinksAsync(linkData);

        // Act
        var result = await ExampleLinkRepository.CheckAnyExampleLinksExistAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeTrue();
    }
}