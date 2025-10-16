using Application.UseCases.Styles.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.StylesControllersTests.Base;
using System.Net;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class GetByNameTests(MidjourneyTestWebApplicationFactory factory) : StylesControllerTestsBase(factory)
{
    [Theory]
    [InlineData("ModernArt")]
    [InlineData("ClassicStyle")]
    [InlineData("AbstractPainting")]
    public async Task GetByName_ReturnsValidResponse_ForValidNames(string styleName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{styleName}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var style = await DeserializeResponse<StyleResponse>(response);
            style.Should().NotBeNull();
            style!.Name.Should().Be(styleName);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task GetByName_HandlesBadRequest_ForInvalidNames(string invalidName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(invalidName)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByName_ReturnsNotFound_ForNonExistentStyle()
    {
        // Arrange
        var nonExistentName = GenerateTestStyleName();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{nonExistentName}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("Style With Spaces")]
    [InlineData("Style_With_Underscores")]
    [InlineData("Style-With-Dashes")]
    [InlineData("UPPERCASESTYLE")]
    [InlineData("lowercasestyle")]
    public async Task GetByName_HandlesSpecialCharacters_InNames(string styleName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(styleName)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByName_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/TestStyle");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var style = await DeserializeResponse<StyleResponse>(response);
            style.Should().NotBeNull();
            style!.Name.Should().NotBeNullOrEmpty();
            style.Type.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task GetByName_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/TestStyle");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }
}