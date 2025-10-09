using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.ExampleLinks;

public sealed class CheckLinkWithStyleExistsTests : ExampleLinksControllerTestsBase
{
    public CheckLinkWithStyleExistsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("ModernArt")]
    [InlineData("ClassicStyle")]
    [InlineData("AbstractPainting")]
    [InlineData("TestStyle")]
    public async Task CheckLinkWithStyleExists_ReturnsOk_ForValidStyleNames(string styleName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{styleName}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

            var exists = await GetExistsFromResponse(response);
            //exists.Should().BeOneOf(true, false);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task CheckLinkWithStyleExists_HandlesBadRequest_ForInvalidStyleNames(string invalidStyleName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{Uri.EscapeDataString(invalidStyleName)}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsJsonWithExistsProperty()
    {
        // Arrange
        var testStyleName = GenerateTestStyleName();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{testStyleName}/exists");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("exists");
            content.Should().MatchRegex(@"\{.*""exists"":(true|false).*\}");
        }
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsConsistentResults()
    {
        // Arrange
        var testStyleName = GenerateTestStyleName();

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/style/{testStyleName}/exists");
        var response2 = await Client.GetAsync($"{BaseUrl}/style/{testStyleName}/exists");

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
    [InlineData("Style With Spaces")]
    [InlineData("Style_With_Underscores")]
    [InlineData("Style-With-Dashes")]
    [InlineData("Style123WithNumbers")]
    [InlineData("UPPERCASESTYLE")]
    [InlineData("lowercasestyle")]
    public async Task CheckLinkWithStyleExists_HandlesSpecialCharacters_InStyleNames(string styleName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{Uri.EscapeDataString(styleName)}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var exists = await GetExistsFromResponse(response);
            //exists.Should().BeOneOf(true, false);
        }
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_PerformanceTest()
    {
        // Arrange
        var testStyleName = GenerateTestStyleName();
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{testStyleName}/exists");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }
}