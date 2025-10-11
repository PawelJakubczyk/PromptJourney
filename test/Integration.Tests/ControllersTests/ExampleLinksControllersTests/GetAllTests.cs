using Application.UseCases.ExampleLinks.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.ExampleLinksControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.ExampleLinks;

public sealed class GetAllTests(MidjourneyTestWebApplicationFactory factory) : ExampleLinksControllerTestsBase(factory)
{
    [Fact]
    public async Task GetAll_ReturnsOk_WithValidResponse()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var links = await DeserializeResponse<List<ExampleLinkResponse>>(response);
        links.Should().NotBeNull();
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

        var links1 = await DeserializeResponse<List<ExampleLinkResponse>>(response1);
        var links2 = await DeserializeResponse<List<ExampleLinkResponse>>(response2);

        links1.Should().BeEquivalentTo(links2);
    }

    [Fact]
    public async Task GetAll_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        AssertOkResponse<ExampleLinkResponse>(response);

        var links = await DeserializeResponse<List<ExampleLinkResponse>>(response);
        links.Should().NotBeNull();

        if (links!.Any())
        {
            var firstLink = links.First();
            firstLink.Link.Should().NotBeNullOrEmpty();
            firstLink.Style.Should().NotBeNullOrEmpty();
            firstLink.Version.Should().NotBeNullOrEmpty();

            // Validate URL format
            Uri.TryCreate(firstLink.Link, UriKind.Absolute, out _).Should().BeTrue();
        }
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

    [Fact]
    public async Task GetAll_ReturnsCorrectContentType()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        //response.Content.Headers.ContentType?.Charset.Should().Be("utf-8");
    }
}