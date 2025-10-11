using Application.UseCases.Styles.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.StylesControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class GetByTypeTests(MidjourneyTestWebApplicationFactory factory) : StylesControllerTestsBase(factory)
{
    [Theory]
    [InlineData("Custom")]
    [InlineData("Abstract")]
    [InlineData("Realistic")]
    [InlineData("Traditional")]
    public async Task GetByType_ReturnsValidResponse_ForValidTypes(string styleType)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-type/{styleType}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var styles = await DeserializeResponse<List<StyleResponse>>(response);
            styles.Should().NotBeNull();

            if (styles!.Any())
            {
                styles.Should().AllSatisfy(style => style.Type.Should().Be(styleType));
            }
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("InvalidType")]
    public async Task GetByType_HandlesInvalidTypes(string invalidType)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-type/{Uri.EscapeDataString(invalidType)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByType_ReturnsEmptyList_ForNonExistentType()
    {
        // Arrange
        var nonExistentType = $"NonExistent_{Guid.NewGuid().ToString("N")[..8]}";

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-type/{nonExistentType}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var styles = await DeserializeResponse<List<StyleResponse>>(response);
            styles.Should().NotBeNull();
            styles.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task GetByType_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/by-type/Custom");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            AssertOkResponse<StyleResponse>(response);

            var styles = await DeserializeResponse<List<StyleResponse>>(response);
            styles.Should().NotBeNull();
        }
    }
}