using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class GetStylesByTypeTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // Test Constants
    private const string TestStyleType1 = "Abstract";

    private const string TestStyleType2 = "Realistic";

    [Fact]
    public async Task GetStylesByTypeAsync_WithExistingType_ShouldReturnMatchingStyles()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1, TestStyleType1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName2, TestStyleType1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName3, TestStyleType2);

        var styleType = StyleType.Create(TestStyleType1).Value;

        // Act
        var result = await StylesRepository.GetStylesByTypeAsync(styleType, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(s => s.Type.Value.Should().Be(TestStyleType1));
        result.Value.Should().Contain(s => s.StyleName.Value == DefaultTestStyleName1);
        result.Value.Should().Contain(s => s.StyleName.Value == DefaultTestStyleName2);
    }

    [Fact]
    public async Task GetStylesByTypeAsync_WithNonExistentType_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1, TestStyleType1);
        var styleType = StyleType.Create("NonExistentType").Value;

        // Act
        var result = await StylesRepository.GetStylesByTypeAsync(styleType, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStylesByTypeAsync_WithEmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange
        var styleType = StyleType.Create(TestStyleType1).Value;

        // Act
        var result = await StylesRepository.GetStylesByTypeAsync(styleType, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStylesByTypeAsync_WithMultipleDifferentTypes_ShouldReturnOnlyMatching()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync("Style1", TestStyleType1);
        await CreateAndSaveTestStyleAsync("Style2", TestStyleType1);
        await CreateAndSaveTestStyleAsync("Style3", TestStyleType2);
        await CreateAndSaveTestStyleAsync("Style4", "Custom");

        var styleType = StyleType.Create(TestStyleType2).Value;

        // Act
        var result = await StylesRepository.GetStylesByTypeAsync(styleType, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(1);
        result.Value.First().StyleName.Value.Should().Be("Style3");
        result.Value.First().Type.Value.Should().Be(TestStyleType2);
    }
}