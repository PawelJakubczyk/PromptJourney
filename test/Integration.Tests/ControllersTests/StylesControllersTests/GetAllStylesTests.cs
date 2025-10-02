using Application.Features.Styles.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class GetAllStylesTests : StylesControllerTestsBase
{
    public GetAllStylesTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithValidResponse()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var styles = await DeserializeResponse<List<StyleResponse>>(response);
        styles.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        AssertOkResponse<StyleResponse>(response);

        var styles = await DeserializeResponse<List<StyleResponse>>(response);
        styles.Should().NotBeNull();

        if (styles!.Any())
        {
            var firstStyle = styles.First();
            firstStyle.Name.Should().NotBeNullOrEmpty();
            firstStyle.Type.Should().NotBeNullOrEmpty();
            // Description can be null
            // Tags can be null or empty
        }
    }

    [Fact]
    public async Task GetAll_ReturnsConsistentResults()
    {
        // Act
        var response1 = await Client.GetAsync(BaseUrl);
        var response2 = await Client.GetAsync(BaseUrl);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        var styles1 = await DeserializeResponse<List<StyleResponse>>(response1);
        var styles2 = await DeserializeResponse<List<StyleResponse>>(response2);

        styles1.Should().BeEquivalentTo(styles2);
    }

    [Fact]
    public async Task GetAll_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}