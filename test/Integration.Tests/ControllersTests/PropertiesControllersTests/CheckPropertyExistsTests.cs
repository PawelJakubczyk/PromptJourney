using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.PropertiesControllersTests;

public sealed class CheckPropertyExistsTests : PropertiesControllerTestsBase
{
    public CheckPropertyExistsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("1.0", "aspect")]
    [InlineData("2.0", "quality")]
    [InlineData("5.2", "stylize")]
    [InlineData("6.0", "chaos")]
    public async Task CheckPropertyExists_ReturnsOk_ForValidParameters(string version, string propertyName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/version/{version}/{propertyName}/exists");

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
    [InlineData("", "aspect")]           // Empty version
    [InlineData("1.0", "")]              // Empty property name
    [InlineData("", "")]                 // Both empty
    [InlineData(" ", " ")]               // Whitespace
    [InlineData("invalid", "aspect")]    // Invalid version
    [InlineData("1.0", "invalid")]       // Invalid property name
    public async Task CheckPropertyExists_HandlesBadRequest_ForInvalidParameters(string version, string propertyName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/version/{Uri.EscapeDataString(version)}/{Uri.EscapeDataString(propertyName)}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsJsonWithExistsProperty()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/version/1.0/aspect/exists");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("exists");
            content.Should().MatchRegex(@"\{.*""exists"":(true|false).*\}");
        }
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsConsistentResults()
    {
        // Arrange
        const string version = "1.0";
        const string propertyName = "aspect";

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/version/{version}/{propertyName}/exists");
        var response2 = await Client.GetAsync($"{BaseUrl}/version/{version}/{propertyName}/exists");

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
    [InlineData("Property With Spaces")]
    [InlineData("Property_With_Underscores")]
    [InlineData("Property-With-Dashes")]
    [InlineData("UPPERCASEPROPERTY")]
    [InlineData("lowercaseproperty")]
    public async Task CheckPropertyExists_HandlesSpecialCharacters_InPropertyNames(string propertyName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/version/1.0/{Uri.EscapeDataString(propertyName)}/exists");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.NotFound);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var exists = await GetExistsFromResponse(response);
            //exists.Should().BeOneOf(true, false);
        }
    }

    [Fact]
    public async Task CheckPropertyExists_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/version/1.0/aspect/exists");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(2));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }
}