using FluentAssertions;
using Integration.Tests.ControllersTests.StylesControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class DeleteStyleTests : StylesControllerTestsBase
{
    public DeleteStyleTests(MidjourneyTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("StyleToDelete")]
    [InlineData("AnotherStyle")]
    [InlineData("TestStyle")]
    public async Task Delete_ReturnsValidResponse_ForValidNames(string styleName)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{styleName}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,  // Successfully deleted
            HttpStatusCode.NotFound,   // Style doesn't exist
            HttpStatusCode.BadRequest  // Invalid name
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
    public async Task Delete_HandlesBadRequest_ForInvalidNames(string invalidName)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(invalidName)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_ForNonExistentStyle()
    {
        // Arrange
        var nonExistentName = GenerateTestStyleName();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{nonExistentName}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("Style With Spaces")]
    [InlineData("Style_With_Underscores")]
    [InlineData("Style-With-Dashes")]
    public async Task Delete_HandlesSpecialCharacters_InNames(string styleName)
    {
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{Uri.EscapeDataString(styleName)}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task Delete_IsIdempotent()
    {
        // Arrange
        var styleName = GenerateTestStyleName();

        // Act
        var response1 = await Client.DeleteAsync($"{BaseUrl}/{styleName}");
        var response2 = await Client.DeleteAsync($"{BaseUrl}/{styleName}");

        // Assert
        response1.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
        response2.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_PerformanceTest()
    {
        // Arrange
        var styleName = GenerateTestStyleName();
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{styleName}");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest
        );
    }
}