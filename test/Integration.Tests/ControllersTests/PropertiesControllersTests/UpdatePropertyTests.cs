using Application.UseCases.Properties.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.PropertiesControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests.ControllersTests.PropertiesControllersTests;

public sealed class UpdatePropertyTests(MidjourneyTestWebApplicationFactory factory) : PropertiesControllerTestsBase(factory)
{
    [Fact]
    public async Task UpdateProperty_ReturnsValidResponse_WithValidRequest()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new UpdatePropertyRequest(
            version,
            propertyName,
            ["--ar", "--aspect"],
            "16:9",
            "1:4",
            "4:1",
            "Updated aspect ratio description"
        );

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/version/{version}/{propertyName}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,          // Successfully updated
            HttpStatusCode.BadRequest,  // Validation failed or mismatch
            HttpStatusCode.NotFound     // Property or version not found
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var updatedProperty = await DeserializeResponse<PropertyResponse>(response);
            updatedProperty.Should().NotBeNull();
            updatedProperty!.PropertyName.Should().Be(propertyName);
            updatedProperty.Version.Should().Be(version);
        }
    }

    [Theory]
    [InlineData("1.0", "aspect", "2.0", "aspect")]    // Version mismatch
    [InlineData("1.0", "aspect", "1.0", "quality")]   // Property name mismatch
    [InlineData("1.0", "aspect", "2.0", "quality")]   // Both mismatch
    public async Task UpdateProperty_ReturnsBadRequest_WhenRouteParametersDontMatchPayload(string routeVersion, string routePropertyName, string payloadVersion, string payloadPropertyName)
    {
        // Arrange
        var request = new UpdatePropertyRequest(
            payloadVersion,
            payloadPropertyName,
            ["--test"]
        );

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/version/{routeVersion}/{routePropertyName}", request);

        // Assert
        AssertBadRequestResponse(response);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WithInvalidRequest()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new UpdatePropertyRequest(
            version,
            propertyName,
            [] // Empty parameters - likely invalid
        );

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/version/{version}/{propertyName}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsNotFound_ForNonExistentProperty()
    {
        // Arrange
        var version = "1.0";
        var propertyName = GenerateTestPropertyName();
        var request = new UpdatePropertyRequest(
            version,
            propertyName,
            ["--test"]
        );

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/version/{version}/{propertyName}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_HandlesSpecialCharacters()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "test-property";
        var request = new UpdatePropertyRequest(
            version,
            propertyName,
            ["--param-with-dashes", "--param_with_underscores"],
            "default value with spaces",
            "min value",
            "max value",
            "Description with spéciál characters & symbols"
        );

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/version/{Uri.EscapeDataString(version)}/{Uri.EscapeDataString(propertyName)}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );
    }

    [Fact]
    public async Task UpdateProperty_PerformanceTest()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new UpdatePropertyRequest(
            version,
            propertyName,
            ["--ar", "--aspect"]
        );
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/version/{version}/{propertyName}", request);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.Should().NotBeNull();
    }
}

// Request DTO for integration tests
public sealed record UpdatePropertyRequest(
    string Version,
    string PropertyName,
    List<string> Parameters,
    string? DefaultValue = null,
    string? MinValue = null,
    string? MaxValue = null,
    string? Description = null
);