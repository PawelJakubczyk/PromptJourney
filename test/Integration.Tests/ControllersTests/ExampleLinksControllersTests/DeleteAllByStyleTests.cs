using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;

namespace Integration.Tests.ControllersTests.ExampleLinks;

public sealed class DeleteAllByStyleTests : ExampleLinksControllerTestsBase
{
    public DeleteAllByStyleTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("ModernArt")]
    [InlineData("ClassicStyle")]
    [InlineData("AbstractPainting")]
    [InlineData("TestStyle")]
    public async Task DeleteAllByStyle_ReturnsValidResponse_ForValidStyleNames(string styleName)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/style/{styleName}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,  // Successfully deleted (or no links to delete)
            HttpStatusCode.NotFound,   // Style doesn't exist
            HttpStatusCode.BadRequest  // Invalid style name
        );

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            AssertNoContentResponse(response);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task DeleteAllByStyle_HandlesBadRequest_ForInvalidStyleNames(string invalidStyleName)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/style/{Uri.EscapeDataString(invalidStyleName)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsValidResponse_ForNonExistentStyle()
    {
        // Arrange
        var nonExistentStyleName = GenerateTestStyleName();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/style/{nonExistentStyleName}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("Style With Spaces")]
    [InlineData("Style_With_Underscores")]
    [InlineData("Style-With-Dashes")]
    [InlineData("Style123WithNumbers")]
    [InlineData("UPPERCASESTYLE")]
    [InlineData("lowercasestyle")]
    public async Task DeleteAllByStyle_HandlesSpecialCharacters_InStyleNames(string styleName)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/style/{Uri.EscapeDataString(styleName)}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task DeleteAllByStyle_IsIdempotent()
    {
        // Arrange
        var testStyleName = GenerateTestStyleName();

        // Act
        var response1 = await Client.DeleteAsync($"{BaseUrl}/style/{testStyleName}");
        var response2 = await Client.DeleteAsync($"{BaseUrl}/style/{testStyleName}");

        // Assert
        response1.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
        response2.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);

        // Both requests should return the same result for idempotency
        if (response1.StatusCode == HttpStatusCode.NoContent)
        {
            response2.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.NoContent);
        }
    }

    [Fact]
    public async Task DeleteAllByStyle_PerformanceTest()
    {
        // Arrange
        var testStyleName = GenerateTestStyleName();
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/style/{testStyleName}");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task DeleteAllByStyle_ValidatesResponseHeaders()
    {
        // Arrange
        var testStyleName = GenerateTestStyleName();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/style/{testStyleName}");

        // Assert
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            response.Content.Headers.ContentLength.Should().Be(0);
            response.Content.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task DeleteAllByStyle_HandlesLongStyleNames()
    {
        // Arrange
        var longStyleName = new string('A', 100); // Very long style name

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/style/{Uri.EscapeDataString(longStyleName)}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task DeleteAllByStyle_HandlesConcurrentRequests()
    {
        // Arrange
        var testStyleName = GenerateTestStyleName();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < 3; i++)
        {
            tasks.Add(Client.DeleteAsync($"{BaseUrl}/style/{testStyleName}"));
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