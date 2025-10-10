using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.ExampleLinksRepositoryTests;

public class CheckExampleLinkExistsTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // CheckExampleLinkExistsAsync Tests
    [Fact]
    public async Task CheckExampleLinkExistsAsync_WithExistingLink_ShouldReturnTrue()
    {
        // Arrange
        await CreateBasicTestDataAsync();
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);

        var link = ExampleLink.Create(DefaultTestLink1).Value;

        // Act
        var result = await ExampleLinkRepository.CheckExampleLinkExistsAsync(link, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckExampleLinkExistsAsync_WithNonExistentLink_ShouldReturnFalse()
    {
        // Arrange
        var link = ExampleLink.Create("https://nonexistent.com/test.jpg").Value;

        // Act
        var result = await ExampleLinkRepository.CheckExampleLinkExistsAsync(link, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckExampleLinkExistsAsync_WithMultipleLinksInDatabase_ShouldFindCorrectOne()
    {
        // Arrange
        var (_, _) = await CreateMultipleTestDataAsync();

        var linkData = new[]
        {
            (DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1),
            (DefaultTestLink2, DefaultTestStyleName2, DefaultTestVersion2),
            (DefaultTestLink3, DefaultTestStyleName3, DefaultTestVersion3)
        };
        await CreateAndSaveMultipleExampleLinksAsync(linkData);

        var searchLink = ExampleLink.Create(DefaultTestLink2).Value;

        // Act
        var result = await ExampleLinkRepository.CheckExampleLinkExistsAsync(searchLink, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeTrue();
    }
}