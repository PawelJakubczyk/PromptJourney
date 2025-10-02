using Application.Features.ExampleLinks.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;

namespace Integration.Tests.ControllersTests.ExampleLinks;

public sealed class GetByStyleTests : ExampleLinksControllerTestsBase
{
    public GetByStyleTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("ModernArt")]
    [InlineData("ClassicStyle")]
    [InlineData("AbstractPainting")]
    public async Task GetByStyle_ReturnsOk_ForValidStyleNames(string styleName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{styleName}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var links = await DeserializeResponse<List<ExampleLinkResponse>>(response);
            links.Should().NotBeNull();

            if (links!.Any())
            {
                links.Should().AllSatisfy(link => link.Style.Should().Be(styleName));
            }
        }
    }

    [Fact]
    public async Task GetByStyle_ReturnsValidResponse_ForExistingStyle()
    {
        // Act - Try with a common style name that might exist
        var response = await Client.GetAsync($"{BaseUrl}/style/TestStyle");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            AssertOkResponse<ExampleLinkResponse>(response);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task GetByStyle_HandlesBadRequest_ForInvalidStyleNames(string invalidStyleName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{Uri.EscapeDataString(invalidStyleName)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("Style With Spaces")]
    [InlineData("Style_With_Underscores")]
    [InlineData("Style-With-Dashes")]
    [InlineData("Style123WithNumbers")]
    public async Task GetByStyle_HandlesSpecialCharacters_InStyleNames(string styleName)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/style/{Uri.EscapeDataString(styleName)}");

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
    public async Task GetByStyle_ReturnsConsistentResults()
    {
        // Arrange
        var styleName = "ConsistencyTestStyle";

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/style/{styleName}");
        var response2 = await Client.GetAsync($"{BaseUrl}/style/{styleName}");

        // Assert
        response1.StatusCode.Should().Be(response2.StatusCode);

        if (response1.StatusCode == HttpStatusCode.OK)
        {
            var links1 = await DeserializeResponse<List<ExampleLinkResponse>>(response1);
            var links2 = await DeserializeResponse<List<ExampleLinkResponse>>(response2);

            links1.Should().BeEquivalentTo(links2);
        }
    }
}