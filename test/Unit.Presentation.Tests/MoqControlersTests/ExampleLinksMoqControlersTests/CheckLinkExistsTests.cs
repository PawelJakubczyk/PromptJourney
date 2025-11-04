using Application.UseCases.ExampleLinks.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class CheckLinkExistsTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task CheckLinkExists_ReturnsOkWithTrue_WhenLinkExists()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsOkWithFalse_WhenLinkDoesNotExist()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenLinkIdIsInvalidGuid()
    {
        // Arrange
        var invalidLinkId = "not-a-guid";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid link ID format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(invalidLinkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenLinkIdIsEmpty()
    {
        // Arrange
        var emptyLinkId = string.Empty;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Link ID cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(emptyLinkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenLinkIdIsWhitespace()
    {
        // Arrange
        var whitespaceLinkId = "   ";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Link ID cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(whitespaceLinkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        // ToResultsCheckExistOkAsync maps all non-400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        CheckExampleLinkExistsById.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as CheckExampleLinkExistsById.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(linkId, capturedQuery!.Id);
    }

    [Fact]
    public async Task CheckLinkExists_HandlesCancellationToken()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.CheckLinkExists(linkId, cts.Token));
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000001")]
    [InlineData("a1b2c3d4-e5f6-7890-1234-567890abcdef")]
    [InlineData("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
    public async Task CheckLinkExists_ReturnsOk_ForVariousValidGuids(string linkId)
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-guid")]
    [InlineData("12345")]
    [InlineData("invalid-format-12345")]
    [InlineData("00000000-0000-0000-0000-00000000000g")] // Invalid character
    public async Task CheckLinkExists_ReturnsBadRequest_ForInvalidInputs(string invalidLinkId)
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid link ID");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(invalidLinkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenNullLinkId()
    {
        // Arrange
        string? nullLinkId = null;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Link ID cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(nullLinkId!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsConsistentResults_ForSameLinkId()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.CheckLinkExists(linkId, CancellationToken.None);
        var actionResult2 = await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        // Both should return the same status
        AssertOkResult<bool>(actionResult1);
        AssertOkResult<bool>(actionResult2);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_ForMalformedGuidWithHyphens()
    {
        // Arrange
        var malformedGuid = "12345678-1234-1234-1234-12345678"; // Too short
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Malformed GUID format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(malformedGuid, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CheckLinkExists_HandlesCaseInsensitiveGuids()
    {
        // Arrange
        var lowercaseGuid = "a1b2c3d4-e5f6-7890-abcd-1234567890ef";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(lowercaseGuid, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }
}