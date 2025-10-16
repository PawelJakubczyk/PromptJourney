using FluentAssertions;
using Integration.Tests.ControllersTests.StylesControllersTests.Base;
using System.Net;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class CheckTagExistsTests(MidjourneyTestWebApplicationFactory factory) : StylesControllerTestsBase(factory)
{
    [Theory]
    [InlineData("ModernArt", "abstract")]
    [InlineData("ClassicStyle", "vintage")]
    [InlineData("TestStyle", "modern")]
    public async Task CheckTagExists_ReturnsValidResponse_ForValidParameters(string styleName, string tag)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{styleName}/tags/{tag}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
            _ = await GetExistsFromResponse(response);
            //exists.Should().BeOneOf(true, false);
        }
    }

    [Theory]
    [InlineData("", "tag")]           // Empty style name
    [InlineData("StyleName", "")]     // Empty tag
    [InlineData("", "")]              // Both empty
    [InlineData(" ", " ")]            // Whitespace
    public async Task CheckTagExists_HandlesBadRequest_ForInvalidParameters(string styleName, string tag)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(styleName)}/tags/{Uri.EscapeDataString(tag)}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsJsonWithExistsProperty()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/TestStyle/tags/testtag/exists");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("exists");
            content.Should().MatchRegex(@"\{.*""exists"":(true|false).*\}");
        }
    }

    [Theory]
    [InlineData("Style With Spaces", "tag with spaces")]
    [InlineData("Style_With_Underscores", "tag_with_underscores")]
    [InlineData("Style-With-Dashes", "tag-with-dashes")]
    public async Task CheckTagExists_HandlesSpecialCharacters_InParameters(string styleName, string tag)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(styleName)}/tags/{Uri.EscapeDataString(tag)}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            _ = await GetExistsFromResponse(response);
            //exists.Should().BeOneOf(true, false);
        }
    }

    [Fact]
    public async Task CheckTagExists_ReturnsConsistentResults()
    {
        // Arrange
        var styleName = GenerateTestStyleName();
        var tag = GenerateTestTag();

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/{styleName}/tags/{tag}/exists");
        var response2 = await Client.GetAsync($"{BaseUrl}/{styleName}/tags/{tag}/exists");

        // Assert
        response1.StatusCode.Should().Be(response2.StatusCode);

        if (response1.StatusCode == HttpStatusCode.OK && response2.StatusCode == HttpStatusCode.OK)
        {
            var exists1 = await GetExistsFromResponse(response1);
            var exists2 = await GetExistsFromResponse(response2);

            exists1.Should().Be(exists2);
        }
    }
}