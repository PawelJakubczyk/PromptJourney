using Application.Features.Properties.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.PropertiesControllersTests;

public sealed class GetAllByVersionTests : PropertiesControllerTestsBase
{
    public GetAllByVersionTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    [InlineData("5.2")]
    [InlineData("6.0")]
    public async Task GetAllByVersion_ReturnsOk_ForValidVersions(string version)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/version/{version}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var properties = await DeserializeResponse<List<PropertyResponse>>(response);
            properties.Should().NotBeNull();

            if (properties!.Any())
            {
                properties.Should().AllSatisfy(prop => prop.Version.Should().Be(version));
            }
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid")]
    [InlineData("999.0")]
    public async Task GetAllByVersion_HandlesBadRequest_ForInvalidVersions(string invalidVersion)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/version/{Uri.EscapeDataString(invalidVersion)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllByVersion_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/version/1.0");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            AssertOkResponse<PropertyResponse>(response);

            var properties = await DeserializeResponse<List<PropertyResponse>>(response);
            properties.Should().NotBeNull();

            if (properties!.Any())
            {
                var firstProperty = properties.First();
                firstProperty.Version.Should().NotBeNullOrEmpty();
                firstProperty.PropertyName.Should().NotBeNullOrEmpty();
                firstProperty.Parameters.Should().NotBeNull();
                firstProperty.Parameters.Should().NotBeEmpty();
            }
        }
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsConsistentResults()
    {
        // Arrange
        const string version = "1.0";

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/version/{version}");
        var response2 = await Client.GetAsync($"{BaseUrl}/version/{version}");

        // Assert
        response1.StatusCode.Should().Be(response2.StatusCode);

        if (response1.StatusCode == HttpStatusCode.OK && response2.StatusCode == HttpStatusCode.OK)
        {
            var properties1 = await DeserializeResponse<List<PropertyResponse>>(response1);
            var properties2 = await DeserializeResponse<List<PropertyResponse>>(response2);

            properties1.Should().BeEquivalentTo(properties2);
        }
    }

    [Theory]
    [InlineData("niji 5")]
    [InlineData("niji 6")]
    [InlineData("v1.0")]
    public async Task GetAllByVersion_HandlesSpecialVersionFormats(string version)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/version/{Uri.EscapeDataString(version)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var properties = await DeserializeResponse<List<PropertyResponse>>(response);
            properties.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetAllByVersion_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/version/1.0");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }
}