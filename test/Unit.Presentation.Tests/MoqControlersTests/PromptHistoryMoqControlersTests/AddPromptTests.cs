using Application.UseCases.PromptHistory.Commands;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests;

public sealed class AddPromptTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task AddPrompt_ReturnsCreatedWithHistoryId_WhenPromptAddedSuccessfully()
    {
        // Arrange
        var request = new AddPromptRequest(
            "A beautiful mountain landscape at sunset",
            "1.0"
        );

        var historyId = Guid.NewGuid().ToString();
        var result = Result.Ok(historyId);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(PromptHistoriesController.GetRecordCount));
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenPromptIsEmpty()
    {
        // Arrange
        var invalidRequest = new AddPromptRequest(
            string.Empty,
            "1.0"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Prompt cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenVersionIsEmpty()
    {
        // Arrange
        var invalidRequest = new AddPromptRequest(
            "Test prompt",
            string.Empty
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenBothFieldsAreEmpty()
    {
        // Arrange
        var invalidRequest = new AddPromptRequest(
            string.Empty,
            string.Empty
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Prompt and version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsConflict_WhenVersionDoesNotExist()
    {
        // Arrange
        var request = new AddPromptRequest(
            "Test prompt",
            "99.0"
        );

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status409Conflict,
            "Version '99.0' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenPromptExceedsMaxLength()
    {
        // Arrange
        var request = new AddPromptRequest(
            new string('a', 10000), // Very long prompt
            "1.0"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Prompt exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenPromptIsWhitespace()
    {
        // Arrange
        var request = new AddPromptRequest(
            "   ",
            "1.0"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Prompt cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var request = new AddPromptRequest(
            "Test prompt",
            "   "
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenPromptIsNull()
    {
        // Arrange
        var request = new AddPromptRequest(
            null!,
            "1.0"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Prompt cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenVersionIsNull()
    {
        // Arrange
        var request = new AddPromptRequest(
            "Test prompt",
            null!
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenVersionFormatIsInvalid()
    {
        // Arrange
        var request = new AddPromptRequest(
            "Test prompt",
            "invalid-version"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid version format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var request = new AddPromptRequest(
            "Test prompt",
            "1.0"
        );

        var failureResult = CreateFailureResult<string, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        // ToResultsCreatedAsync maps all non-409/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var request = new AddPromptRequest("Test prompt", "1.0");
        var historyId = Guid.NewGuid().ToString();
        var result = Result.Ok(historyId);
        var senderMock = new Mock<ISender>();
        AddPromptToHistory.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<string>>, CancellationToken>((cmd, ct) =>
            {
                capturedCommand = cmd as AddPromptToHistory.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(request.Prompt, capturedCommand!.Prompt);
        Assert.Equal(request.Version, capturedCommand.Version);
    }

    [Fact]
    public async Task AddPrompt_HandlesCancellationToken()
    {
        // Arrange
        var request = new AddPromptRequest("Test prompt", "1.0");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.AddPrompt(request, cts.Token));
    }

    [Fact]
    public async Task AddPrompt_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var request = new AddPromptRequest("Test prompt", "1.0");
        var historyId = Guid.NewGuid().ToString();
        var result = Result.Ok(historyId);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("A beautiful landscape", "1.0")]
    [InlineData("Modern city at night", "2.0")]
    [InlineData("Abstract art style", "5.2")]
    [InlineData("Minimal design", "6.0")]
    public async Task AddPrompt_ReturnsCreated_ForVariousValidInputs(string prompt, string version)
    {
        // Arrange
        var request = new AddPromptRequest(prompt, version);
        var historyId = Guid.NewGuid().ToString();
        var result = Result.Ok(historyId);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(PromptHistoriesController.GetRecordCount));
    }

    [Fact]
    public async Task AddPrompt_ReturnsCreated_WithVeryLongValidPrompt()
    {
        // Arrange
        var longPrompt = new string('a', 2000); // Long but valid prompt
        var request = new AddPromptRequest(longPrompt, "1.0");
        var historyId = Guid.NewGuid().ToString();
        var result = Result.Ok(historyId);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(PromptHistoriesController.GetRecordCount));
    }

    [Fact]
    public async Task AddPrompt_ReturnsCreated_WithSpecialCharactersInPrompt()
    {
        // Arrange
        var promptWithSpecialChars = "A @#$% beautiful !@# landscape & sunset";
        var request = new AddPromptRequest(promptWithSpecialChars, "1.0");
        var historyId = Guid.NewGuid().ToString();
        var result = Result.Ok(historyId);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(PromptHistoriesController.GetRecordCount));
    }

    [Fact]
    public async Task AddPrompt_ReturnsConsistentResults_ForSameInput()
    {
        // Arrange
        var request = new AddPromptRequest("Test prompt", "1.0");
        var historyId1 = Guid.NewGuid().ToString();
        var historyId2 = Guid.NewGuid().ToString();
        var senderMock = new Mock<ISender>();

        senderMock
            .SetupSequence(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(historyId1))
            .ReturnsAsync(Result.Ok(historyId2));

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.AddPrompt(request, CancellationToken.None);
        var actionResult2 = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult1, nameof(PromptHistoriesController.GetRecordCount));
        AssertCreatedResult<string>(actionResult2, nameof(PromptHistoriesController.GetRecordCount));
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var request = new AddPromptRequest("Test prompt", "1.0");
        var failureResult = CreateFailureResult<string, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var request = new AddPromptRequest("Test prompt", "1.0");
        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddPrompt_ReturnsCreated_WithUnicodeCharacters()
    {
        // Arrange
        var promptWithUnicode = "A beautiful ?? landscape ?? ??";
        var request = new AddPromptRequest(promptWithUnicode, "1.0");
        var historyId = Guid.NewGuid().ToString();
        var result = Result.Ok(historyId);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(PromptHistoriesController.GetRecordCount));
    }

    [Fact]
    public async Task AddPrompt_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var request = new AddPromptRequest("Test prompt", "1.0");
        var historyId = Guid.NewGuid().ToString();
        var result = Result.Ok(historyId);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddPromptToHistory.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.AddPrompt(request, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}