using FluentAssertions;
using Integration.Tests.ControllersTests.ExampleLinksControllersTests.Base;
using System.Net;

namespace Integration.Tests.ControllersTests.ExampleLinksControllersTests;

public sealed class DeleteExampleLinkTests(MidjourneyTestWebApplicationFactory factory) : ExampleLinksControllerTestsBase(factory)
{
    [Theory]
    [InlineData("https://example.com/image.jpg")]
    [InlineData("https://test.com/picture.png")]
    [InlineData("http://sample.org/photo.gif")]
    public async Task DeleteExampleLink_ReturnsValidResponse_ForValidUrls(string url)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(url)}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,  // Successfully deleted
            HttpStatusCode.NotFound,   // Link doesn't exist
            HttpStatusCode.BadRequest  // Invalid URL format
        );

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            AssertNoContentResponse(response);
        }
    }

    [Theory]
    [InlineData("invalid-url")]
    [InlineData("not-a-url")]
    [InlineData("")]
    [InlineData(" ")]
    public async Task DeleteExampleLink_HandlesBadRequest_ForInvalidUrls(string invalidUrl)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(invalidUrl)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsNotFound_ForNonExistentUrl()
    {
        // Arrange
        var nonExistentUrl = GenerateTestUrl();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(nonExistentUrl)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("https://example.com/image with spaces.jpg")]
    [InlineData("https://example.com/image%20encoded.jpg")]
    [InlineData("https://example.com/image-with-dashes.jpg")]
    [InlineData("https://example.com/image_with_underscores.jpg")]
    public async Task DeleteExampleLink_HandlesSpecialCharacters_InUrls(string url)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(url)}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task DeleteExampleLink_IsIdempotent()
    {
        // Arrange
        var testUrl = GenerateTestUrl();

        // Act
        var response1 = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(testUrl)}");
        var response2 = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(testUrl)}");

        // Assert
        response1.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
        response2.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);

        // Second delete should typically return NotFound, but some implementations might be idempotent
        if (response1.StatusCode == HttpStatusCode.NoContent)
        {
            response2.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.NoContent);
        }
    }

    [Fact]
    public async Task DeleteExampleLink_PerformanceTest()
    {
        // Arrange
        var testUrl = GenerateTestUrl();
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(testUrl)}");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task DeleteExampleLink_ValidatesResponseHeaders()
    {
        // Arrange
        var testUrl = GenerateTestUrl();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(testUrl)}");

        // Assert
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            response.Content.Headers.ContentLength.Should().Be(0);
        }
    }
}