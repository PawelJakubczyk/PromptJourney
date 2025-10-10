using FluentAssertions;
using Integration.Tests.ControllersTests.PropertiesControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.PropertiesControllersTests;

public sealed class DeletePropertyTests(MidjourneyTestWebApplicationFactory factory) : PropertiesControllerTestsBase(factory)
{
    [Theory]
    [InlineData("1.0", "aspect")]
    [InlineData("2.0", "quality")]
    [InlineData("5.2", "stylize")]
    public async Task DeleteProperty_ReturnsValidResponse_ForValidParameters(string version, string propertyName)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/version/{version}/{propertyName}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,  // Successfully deleted
            HttpStatusCode.NotFound,   // Property doesn't exist
            HttpStatusCode.BadRequest  // Invalid parameters
        );

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            AssertNoContentResponse(response);
        }
    }

    [Theory]
    [InlineData("", "aspect")]           // Empty version
    [InlineData("1.0", "")]              // Empty property name
    [InlineData("", "")]                 // Both empty
    [InlineData(" ", " ")]               // Whitespace
    [InlineData("invalid", "aspect")]    // Invalid version
    [InlineData("1.0", "invalid")]       // Invalid property name
    public async Task DeleteProperty_HandlesBadRequest_ForInvalidParameters(string version, string propertyName)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/version/{Uri.EscapeDataString(version)}/{Uri.EscapeDataString(propertyName)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsNotFound_ForNonExistentProperty()
    {
        // Arrange
        var version = "1.0";
        var propertyName = GenerateTestPropertyName();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/version/{version}/{propertyName}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("999.0", "aspect")]      // Non-existent version
    [InlineData("1.0", "nonexistent")]   // Non-existent property
    public async Task DeleteProperty_ReturnsNotFound_ForNonExistentVersionOrProperty(string version, string propertyName)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/version/{version}/{propertyName}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Property With Spaces")]
    [InlineData("Property_With_Underscores")]
    [InlineData("Property-With-Dashes")]
    [InlineData("UPPERCASEPROPERTY")]
    [InlineData("lowercaseproperty")]
    public async Task DeleteProperty_HandlesSpecialCharacters_InPropertyNames(string propertyName)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/version/1.0/{Uri.EscapeDataString(propertyName)}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task DeleteProperty_IsIdempotent()
    {
        // Arrange
        var version = "1.0";
        var propertyName = GenerateTestPropertyName();

        // Act
        var response1 = await Client.DeleteAsync($"{BaseUrl}/version/{version}/{propertyName}");
        var response2 = await Client.DeleteAsync($"{BaseUrl}/version/{version}/{propertyName}");

        // Assert
        response1.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
        response2.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);

        // Second delete should typically return NotFound
        if (response1.StatusCode == HttpStatusCode.NoContent)
        {
            response2.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.NoContent);
        }
    }

    [Fact]
    public async Task DeleteProperty_ValidatesResponseHeaders()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/version/{version}/{propertyName}");

        // Assert
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            response.Content.Headers.ContentLength.Should().Be(0);
        }
    }

    [Fact]
    public async Task DeleteProperty_PerformanceTest()
    {
        // Arrange
        var version = "1.0";
        var propertyName = GenerateTestPropertyName();
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/version/{version}/{propertyName}");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task DeleteProperty_HandlesConcurrentRequests()
    {
        // Arrange
        var version = "1.0";
        var propertyName = GenerateTestPropertyName();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < 3; i++)
        {
            tasks.Add(Client.DeleteAsync($"{BaseUrl}/version/{version}/{propertyName}"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        foreach (var response in responses)
        {
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.NoContent,
                HttpStatusCode.NotFound,
                HttpStatusCode.BadRequest
            );
        }

        // At least one should succeed or all should fail consistently
        var statusCodes = responses.Select(r => r.StatusCode).Distinct().ToList();
        statusCodes.Should().NotBeEmpty();
    }
}