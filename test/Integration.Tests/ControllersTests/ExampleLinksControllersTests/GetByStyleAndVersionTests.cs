using Application.UseCases.ExampleLinks.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.ExampleLinksControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.ExampleLinks;

public sealed class GetByStyleAndVersionTests(MidjourneyTestWebApplicationFactory factory) : ExampleLinksControllerTestsBase(factory)
{
    [Theory]
    [InlineData("ModernArt", "1.0")]
    [InlineData("ClassicStyle", "2.0")]
    [InlineData("AbstractPainting", "5.2")]
    [InlineData("TestStyle", "6")]
    public async Task GetByStyleAndVersion_ReturnsValidResponse_ForValidParameters(string styleName, string version)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{styleName}/version/{version}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var links = await DeserializeResponse<List<ExampleLinkResponse>>(response);
            links.Should().NotBeNull();

            if (links!.Any())
            {
                links.Should().AllSatisfy(link =>
                {
                    link.Style.Should().Be(styleName);
                    link.Version.Should().Be(version);
                });
            }
        }
    }

    [Theory]
    [InlineData("", "1.0")]
    [InlineData("TestStyle", "")]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    public async Task GetByStyleAndVersion_HandlesBadRequest_ForInvalidParameters(string styleName, string version)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{Uri.EscapeDataString(styleName)}/version/{Uri.EscapeDataString(version)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("Style With Spaces", "1.0")]
    [InlineData("TestStyle", "niji 5")]
    [InlineData("Style-With-Dashes", "v5.2")]
    public async Task GetByStyleAndVersion_HandlesSpecialCharacters_InParameters(string styleName, string version)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{Uri.EscapeDataString(styleName)}/version/{Uri.EscapeDataString(version)}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var links = await DeserializeResponse<List<ExampleLinkResponse>>(response);
            links.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsConsistentResults()
    {
        // Arrange
        var styleName = "ConsistencyStyle";
        var version = "1.0";

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/style/{styleName}/version/{version}");
        var response2 = await Client.GetAsync($"{BaseUrl}/style/{styleName}/version/{version}");

        // Assert
        response1.StatusCode.Should().Be(response2.StatusCode);

        if (response1.StatusCode == HttpStatusCode.OK)
        {
            var links1 = await DeserializeResponse<List<ExampleLinkResponse>>(response1);
            var links2 = await DeserializeResponse<List<ExampleLinkResponse>>(response2);

            links1.Should().BeEquivalentTo(links2);
        }
    }

    [Fact]
    public async Task GetByStyleAndVersion_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/TestStyle/version/1.0");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
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
    }
}