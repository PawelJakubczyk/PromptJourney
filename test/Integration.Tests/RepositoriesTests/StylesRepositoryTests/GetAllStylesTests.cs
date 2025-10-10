using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class GetAllStylesTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // Test Constants
    private const string TestStyleType1 = "Abstract";

    private const string TestStyleType2 = "Realistic";
    private const string TestStyleType3 = "Minimalist";

    [Fact]
    public async Task GetAllStylesAsync_WithMultipleStyles_ShouldReturnAllStyles()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1, TestStyleType1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName2, TestStyleType2);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName3, TestStyleType3);

        // Act
        var result = await StylesRepository.GetAllStylesAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(s => s.StyleName.Value == DefaultTestStyleName1);
        result.Value.Should().Contain(s => s.StyleName.Value == DefaultTestStyleName2);
        result.Value.Should().Contain(s => s.StyleName.Value == DefaultTestStyleName3);
    }

    [Fact]
    public async Task GetAllStylesAsync_WithNoStyles_ShouldReturnEmptyList()
    {
        // Act
        var result = await StylesRepository.GetAllStylesAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllStylesAsync_WithStylesOfDifferentTypes_ShouldReturnAllStyles()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync("Style1", "Custom");
        await CreateAndSaveTestStyleAsync("Style2", "Preset");
        await CreateAndSaveTestStyleAsync("Style3", "User");

        // Act
        var result = await StylesRepository.GetAllStylesAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(s => s.Type.Value == "Custom");
        result.Value.Should().Contain(s => s.Type.Value == "Preset");
        result.Value.Should().Contain(s => s.Type.Value == "User");
    }
}