using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests;

public sealed class GetRecordCountPromptHistoryTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task GetRecordCount_ReturnsOk_WhenRecordsExist()
    {
        // Arrange
        var count = 42;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        var returnedValue = okResult!.Value;

        // Verify the response structure matches { count = 42 }
        returnedValue.Should().NotBeNull();
        var json = System.Text.Json.JsonSerializer.Serialize(returnedValue);
        json.Should().Contain("\"count\":42");
    }

    [Fact]
    public async Task GetRecordCount_ReturnsZero_WhenNoRecordsExist()
    {
        // Arrange
        var count = 0;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        var returnedValue = okResult!.Value;
        var json = System.Text.Json.JsonSerializer.Serialize(returnedValue);
        json.Should().Contain("\"count\":0");
    }

    [Fact]
    public async Task GetRecordCount_ReturnsInternalServerError_WhenHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<int, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task GetRecordCount_ReturnsLargeNumbers_WhenManyRecordsExist()
    {
        // Arrange
        var count = 1000000; // Large number
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        var returnedValue = okResult!.Value;
        var json = System.Text.Json.JsonSerializer.Serialize(returnedValue);
        json.Should().Contain("\"count\":1000000");
    }

    [Fact]
    public async Task GetRecordCount_VerifiesCorrectQueryIsSent()
    {
        // Arrange
        var count = 5;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetRecordCount(CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(
            It.IsAny<Application.UseCases.PromptHistory.Queries.CalculateHistoricalRecordCount.Query>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetRecordCount_HandlesCancellationTokenCorrectly()
    {
        // Arrange
        var count = 10;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        var cancellationToken = new CancellationToken(true); // Cancelled token

        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetRecordCount(cancellationToken);

        // Assert
        senderMock.Verify(s => s.Send(
            It.IsAny<object>(),
            cancellationToken),
            Times.Once);
    }
}
