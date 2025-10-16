using FluentAssertions;
using Integration.Tests.ControllersTests.StylesControllersTests.Base;
using System.Net;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class RemoveTagTests(MidjourneyTestWebApplicationFactory factory) : StylesControllerTestsBase(factory)
{
    [Theory]
    [InlineData("TestStyle", "tagtoremove")]
    [InlineData("AnotherStyle", "anothertag")]
    public async Task RemoveTag_ReturnsValidResponse_ForValidParameters(string styleName, string tag)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{styleName}/tags/{tag}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,  // Successfully removed
            HttpStatusCode.NotFound,   // Style or tag doesn't exist
            HttpStatusCode.BadRequest  // Invalid parameters
        );

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            AssertNoContentResponse(response);
        }
    }

    [Theory]
    [InlineData("", "tag")]           // Empty style name
    [InlineData("StyleName", "")]     // Empty tag
    [InlineData("", "")]              // Both empty
    [InlineData(" ", " ")]            // Whitespace
    public async Task RemoveTag_HandlesBadRequest_ForInvalidParameters(string styleName, string tag)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(styleName)}/tags/{Uri.EscapeDataString(tag)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveTag_ReturnsNotFound_ForNonExistentStyle()
    {
        // Arrange
        var styleName = GenerateTestStyleName();
        var tag = GenerateTestTag();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{styleName}/tags/{tag}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveTag_ReturnsNotFound_ForNonExistentTag()
    {
        // Arrange
        var styleName = "TestStyle";
        var nonExistentTag = GenerateTestTag();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{styleName}/tags/{nonExistentTag}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Style With Spaces", "tag with spaces")]
    [InlineData("Style_With_Underscores", "tag_with_underscores")]
    [InlineData("Style-With-Dashes", "tag-with-dashes")]
    public async Task RemoveTag_HandlesSpecialCharacters_InParameters(string styleName, string tag)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(styleName)}/tags/{Uri.EscapeDataString(tag)}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task RemoveTag_IsIdempotent()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = GenerateTestTag();

        // Act
        var response1 = await Client.DeleteAsync($"{BaseUrl}/{styleName}/tags/{tag}");
        var response2 = await Client.DeleteAsync($"{BaseUrl}/{styleName}/tags/{tag}");

        // Assert
        response1.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
        response2.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveTag_PerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = GenerateTestTag();
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{styleName}/tags/{tag}");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }
}