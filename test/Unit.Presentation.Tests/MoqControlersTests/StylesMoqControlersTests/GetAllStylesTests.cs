using Application.UseCases.Styles.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class GetAllStylesTests : StylesControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsOk_WhenStylesExist()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("ModernArt", "Custom", "Modern art style", ["abstract", "contemporary"]),
            new("ClassicArt", "Traditional", "Classic art style", ["vintage", "traditional"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoStylesExist()
    {
        // Arrange
        var emptyList = new List<StyleResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<StyleResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid request");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsInternalServerError_WhenDatabaseError()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<StyleResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task GetAll_VerifiesCorrectQueryIsSent()
    {
        // Arrange
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(
            It.IsAny<Application.UseCases.Styles.Queries.GetAllStyles.Query>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_HandlesCancellationTokenCorrectly()
    {
        // Arrange
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        var cancellationToken = new CancellationToken(true); // Cancelled token

        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(cancellationToken);

        // Assert
        senderMock.Verify(s => s.Send(
            It.IsAny<object>(),
            cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsStylesWithNullTags_WhenStylesHaveNoTags()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("MinimalStyle", "Modern", "Simple minimal style", null),
            new("CleanStyle", "Contemporary", null, null)
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 2);

        var okResult = actionResult as OkObjectResult;
        var returnedStyles = okResult!.Value as List<StyleResponse>;
        returnedStyles!.Should().Contain(s => s.Name == "MinimalStyle" && s.Tags == null);
        returnedStyles.Should().Contain(s => s.Name == "CleanStyle" && s.Description == null && s.Tags == null);
    }

    [Fact]
    public async Task GetAll_ReturnsLargeNumberOfStyles_WhenManyStylesExist()
    {
        // Arrange
        var styles = new List<StyleResponse>();
        for (int i = 1; i <= 100; i++)
        {
            styles.Add(new StyleResponse(
                $"Style{i}",
                i % 2 == 0 ? "Modern" : "Traditional",
                $"Description for style {i}",
                [$"tag{i}", $"category{i % 5}"]
            ));
        }

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 100);
    }

    [Fact]
    public async Task GetAll_ReturnsStylesWithVariousTypes_WhenDifferentTypesExist()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("ArtStyle1", "Abstract", "Abstract art style", ["abstract", "modern"]),
            new("ArtStyle2", "Realistic", "Realistic art style", ["realistic", "detailed"]),
            new("ArtStyle3", "Minimalist", "Minimal design style", ["minimal", "clean"]),
            new("ArtStyle4", "Vintage", "Retro vintage style", ["vintage", "retro"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 4);

        var okResult = actionResult as OkObjectResult;
        var returnedStyles = okResult!.Value as List<StyleResponse>;
        returnedStyles!.Should().Contain(s => s.Type == "Abstract");
        returnedStyles.Should().Contain(s => s.Type == "Realistic");
        returnedStyles.Should().Contain(s => s.Type == "Minimalist");
        returnedStyles.Should().Contain(s => s.Type == "Vintage");
    }
}
