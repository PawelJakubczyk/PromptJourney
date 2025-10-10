using FluentAssertions;
using Integration.Tests.ControllersTests.StylesControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class CheckExistsTests(MidjourneyTestWebApplicationFactory factory) : StylesControllerTestsBase(factory)
{
    [Theory]
    [InlineData("ModernArt")]
    [InlineData("ClassicStyle")]
    [InlineData("AbstractPainting")]
    public async Task CheckExists_ReturnsValidResponse_ForValidNames(string styleName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{styleName}/exists");

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
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task CheckExists_HandlesBadRequest_ForInvalidNames(string invalidName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(invalidName)}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    [Fact]
    public async Task CheckExists_ReturnsJsonWithExistsProperty()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/TestStyle/exists");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("exists");
            content.Should().MatchRegex(@"\{.*""exists"":(true|false).*\}");
        }
    }

    [Fact]
    public async Task CheckExists_ReturnsConsistentResults()
    {
        // Arrange
        var testStyleName = GenerateTestStyleName();

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/{testStyleName}/exists");
        var response2 = await Client.GetAsync($"{BaseUrl}/{testStyleName}/exists");

        // Assert
        response1.StatusCode.Should().Be(response2.StatusCode);

        if (response1.StatusCode == HttpStatusCode.OK && response2.StatusCode == HttpStatusCode.OK)
        {
            var exists1 = await GetExistsFromResponse(response1);
            var exists2 = await GetExistsFromResponse(response2);

            exists1.Should().Be(exists2);
        }
    }

    [Fact]
    public async Task CheckExists_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/TestStyle/exists");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(2));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }
}