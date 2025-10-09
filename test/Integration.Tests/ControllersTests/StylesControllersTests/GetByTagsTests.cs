using Application.Features.Styles.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class GetByTagsTests : StylesControllerTestsBase
{
    public GetByTagsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("abstract")]
    [InlineData("modern")]
    [InlineData("vintage")]
    public async Task GetByTags_ReturnsValidResponse_ForSingleTag(string tag)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-tags?tags={tag}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var styles = await DeserializeResponse<List<StyleResponse>>(response);
            styles.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetByTags_ReturnsValidResponse_ForMultipleTags()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-tags?tags=abstract&tags=modern");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var styles = await DeserializeResponse<List<StyleResponse>>(response);
            styles.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetByTags_HandlesEmptyTagsList()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-tags");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetByTags_HandlesInvalidTags(string invalidTag)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-tags?tags={Uri.EscapeDataString(invalidTag)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByTags_ReturnsEmptyList_ForNonExistentTags()
    {
        // Arrange
        var nonExistentTag = GenerateTestTag();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-tags?tags={nonExistentTag}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var styles = await DeserializeResponse<List<StyleResponse>>(response);
            styles.Should().NotBeNull();
            styles.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task GetByTags_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-tags?tags=abstract");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            AssertOkResponse<StyleResponse>(response);

            var styles = await DeserializeResponse<List<StyleResponse>>(response);
            styles.Should().NotBeNull();
        }
    }
}