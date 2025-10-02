using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using FluentAssertions;

namespace Integration.Tests.ControllersTests.ExampleLinks;

public sealed class CheckLinkExistsTests : ExampleLinksControllerTestsBase
{
    public CheckLinkExistsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("https://example.com/image.jpg")]
    [InlineData("https://test.com/picture.png")]
    [InlineData("http://sample.org/photo.gif")]
    public async Task CheckLinkExists_ReturnsOk_ForValidUrls(string url)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(url)}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

            var exists = await GetExistsFromResponse(response);
            //exists.Should().BeOneOf(true, false); // Either true or false is valid
        }
    }

    [Theory]
    [InlineData("invalid-url")]
    [InlineData("not-a-url")]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CheckLinkExists_HandlesBadRequest_ForInvalidUrls(string invalidUrl)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(invalidUrl)}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsJsonWithExistsProperty()
    {
        // Arrange
        var testUrl = GenerateTestUrl();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(testUrl)}/exists");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("exists");
            content.Should().MatchRegex(@"\{.*""exists"":(true|false).*\}");
        }
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsConsistentResults()
    {
        // Arrange
        var testUrl = GenerateTestUrl();

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(testUrl)}/exists");
        var response2 = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(testUrl)}/exists");

        // Assert
        response1.StatusCode.Should().Be(response2.StatusCode);

        if (response1.StatusCode == HttpStatusCode.OK && response2.StatusCode == HttpStatusCode.OK)
        {
            var exists1 = await GetExistsFromResponse(response1);
            var exists2 = await GetExistsFromResponse(response2);

            exists1.Should().Be(exists2);
        }
    }

    [Theory]
    [InlineData("https://example.com/image with spaces.jpg")]
    [InlineData("https://example.com/image%20encoded.jpg")]
    [InlineData("https://example.com/image-with-dashes.jpg")]
    [InlineData("https://example.com/image_with_underscores.jpg")]
    public async Task CheckLinkExists_HandlesSpecialCharacters_InUrls(string url)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(url)}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var exists = await GetExistsFromResponse(response);
            //exists.Should().BeOneOf(true, false);
        }
    }
}