using Application.Features.PromptHistory.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests;

public sealed class AddPromptTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task AddPrompt_ReturnsCreated_WhenPromptAddedSuccessfully()
    {
        // Arrange
        var request = new AddPromptRequest(
            "A beautiful mountain landscape at sunset",
            "1.0"
        );

        var response = new PromptHistoryResponse(Guid.NewGuid(), request.Prompt, request.Version, DateTime.UtcNow);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertCreatedResult<PromptHistoryResponse>(actionResult, nameof(PromptHistoryController.GetRecordCount));
    }

    [Fact]
    public async Task AddPrompt_ReturnsNoContent_WhenResultIsNull()
    {
        // Arrange
        var request = new AddPromptRequest(
            "Test prompt",
            "1.0"
        );

        PromptHistoryResponse? nullResponse = null;
        var result = Result.Ok(nullResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenRequestInvalid()
    {
        // Arrange
        var invalidRequest = new AddPromptRequest(
            "",
            ""
        );

        var failureResult = CreateFailureResult<PromptHistoryResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Prompt and version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var request = new AddPromptRequest(
            "Test prompt",
            "99.0"
        );

        var failureResult = CreateFailureResult<PromptHistoryResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Version not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenPromptTooLong()
    {
        // Arrange
        var request = new AddPromptRequest(
            new string('a', 10000), // Very long prompt
            "1.0"
        );

        var failureResult = CreateFailureResult<PromptHistoryResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Prompt exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_VerifiesCorrectCommandIsSent()
    {
        // Arrange
        var request = new AddPromptRequest("Test prompt", "1.0");
        var response = new PromptHistoryResponse(Guid.NewGuid(), request.Prompt, request.Version, DateTime.UtcNow);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(
            It.Is<Application.Features.PromptHistory.Commands.AddPromptToHistory.Command>(
                c => c.Prompt == request.Prompt && c.Version == request.Version),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddPrompt_CreatesCorrectLocationHeader()
    {
        // Arrange
        var request = new AddPromptRequest("Test prompt", "1.0");
        var response = new PromptHistoryResponse(Guid.NewGuid(), request.Prompt, request.Version, DateTime.UtcNow);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = actionResult as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(PromptHistoryController.GetRecordCount));
        createdResult.Value.Should().Be(response);
    }
}
