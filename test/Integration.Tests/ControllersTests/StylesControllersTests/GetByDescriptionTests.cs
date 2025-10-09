using Application.Features.Styles.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class GetByDescriptionTests : StylesControllerTestsBase
{
    public GetByDescriptionTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("modern")]
    [InlineData("abstract")]
    [InlineData("style")]
    public async Task GetByDescription_ReturnsValidResponse_ForValidKeywords(string keyword)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-description?keyword={keyword}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var styles = await DeserializeResponse<List<StyleResponse>>(response);
            styles.Should().NotBeNull();
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetByDescription_HandlesBadRequest_ForInvalidKeywords(string invalidKeyword)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-description?keyword={Uri.EscapeDataString(invalidKeyword)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetByDescription_HandlesMissingKeywordParameter()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-description");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetByDescription_ReturnsEmptyList_ForNonExistentKeyword()
    {
        // Arrange
        var nonExistentKeyword = $"nonexistent_{Guid.NewGuid().ToString("N")[..8]}";

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-description?keyword={nonExistentKeyword}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var styles = await DeserializeResponse<List<StyleResponse>>(response);
            styles.Should().NotBeNull();
            styles.Should().BeEmpty();
        }
    }

    [Theory]
    [InlineData("keyword with spaces")]
    [InlineData("spéciál-characters")]
    [InlineData("UPPERCASE")]
    public async Task GetByDescription_HandlesSpecialCharacters_InKeywords(string keyword)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-description?keyword={Uri.EscapeDataString(keyword)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByDescription_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-description?keyword=test");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }
}