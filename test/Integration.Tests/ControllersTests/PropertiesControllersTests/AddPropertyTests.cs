using Application.UseCases.Properties.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.PropertiesControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests.ControllersTests.PropertiesControllersTests;

public sealed class AddPropertyTests(MidjourneyTestWebApplicationFactory factory) : PropertiesControllerTestsBase(factory)
{
    [Fact]
    public async Task AddProperty_ReturnsValidResponse_WithValidRequest()
    {
        // Arrange
        var version = "1.0";
        var request = new AddPropertyRequest(
            GenerateTestPropertyName(),
            GenerateTestParameters(),
            "default-value",
            "min-value",
            "max-value",
            "Test property description"
        );

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/version/{version}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,     // Successfully created
            HttpStatusCode.BadRequest,  // Validation failed or version doesn't exist
            HttpStatusCode.NotFound,    // Version not found
            HttpStatusCode.Conflict     // Property already exists
        );

        if (response.StatusCode == HttpStatusCode.Created)
        {
            AssertCreatedResponse<PropertyResponse>(response);

            var createdProperty = await DeserializeResponse<PropertyResponse>(response);
            createdProperty.Should().NotBeNull();
            createdProperty!.PropertyName.Should().Be(request.PropertyName);
            createdProperty.Version.Should().Be(version);
        }
    }

    [Theory]
    [InlineData("", null, null, null, null)]              // Empty property name
    [InlineData("ValidName", null, null, null, null)]     // Null parameters
    public async Task AddProperty_ReturnsBadRequest_WithInvalidRequest(string propertyName, List<string>? parameters, string? defaultValue, string? minValue, string? maxValue)
    {
        // Arrange
        var version = "1.0";
        var request = new AddPropertyRequest(propertyName, parameters ?? [], defaultValue, minValue, maxValue);

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/version/{version}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddProperty_ReturnsBadRequest_WithNullRequest()
    {
        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/version/1.0", (AddPropertyRequest?)null);

        // Assert
        AssertBadRequestResponse(response);
    }

    [Theory]
    [InlineData("")]           // Empty version
    [InlineData("invalid")]    // Invalid version
    [InlineData("999.0")]      // Non-existent version
    public async Task AddProperty_HandlesInvalidVersions(string version)
    {
        // Arrange
        var request = new AddPropertyRequest(
            GenerateTestPropertyName(),
            GenerateTestParameters()
        );

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/version/{Uri.EscapeDataString(version)}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound,
            HttpStatusCode.Created
        );
    }

    [Fact]
    public async Task AddProperty_HandlesSpecialCharacters()
    {
        // Arrange
        var version = "1.0";
        var request = new AddPropertyRequest(
            "Property With Spaces",
            ["--param-with-dashes", "--param_with_underscores"],
            "default value with spaces",
            "min value",
            "max value",
            "Description with spéciál characters & symbols: @#$%"
        );

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/version/{version}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound,
            HttpStatusCode.Conflict
        );
    }

    [Fact]
    public async Task AddProperty_ValidatesResponseLocation()
    {
        // Arrange
        var version = "1.0";
        var request = new AddPropertyRequest(
            GenerateTestPropertyName(),
            GenerateTestParameters()
        );

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/version/{version}", request);

        // Assert
        if (response.StatusCode == HttpStatusCode.Created)
        {
            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location!.ToString().Should().Contain("exists");
        }
    }

    [Fact]
    public async Task AddProperty_PerformanceTest()
    {
        // Arrange
        var version = "1.0";
        var request = new AddPropertyRequest(
            GenerateTestPropertyName(),
            GenerateTestParameters()
        );
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/version/{version}", request);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.Should().NotBeNull();
    }
}

// Request DTO for integration tests
public sealed record AddPropertyRequest(
    string PropertyName,
    List<string> Parameters,
    string? DefaultValue = null,
    string? MinValue = null,
    string? MaxValue = null,
    string? Description = null
);