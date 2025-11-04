using Application.UseCases.ExampleLinks.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class AddExampleLinkTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task AddExampleLink_ReturnsCreated_WhenLinkAddedSuccessfully()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "ModernArt",
            "1.0"
        );

        // Handler returns Result<string> (the new link ID)
        var result = Result.Ok("generated-link-id-123");
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertCreatedResult<string>(actionResult, nameof(ExampleLinksController.CheckLinkExists));
    }

    [Fact]
    public async Task AddExampleLink_ReturnsBadRequest_WhenLinkFormatIsInvalid()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "not-a-valid-url",
            "ModernArt",
            "1.0"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid link format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsBadRequest_WhenStyleNameIsInvalid()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "ModernArt_ModernArt_ModernArt_ModernArt_ModernArt_ModernArt_ModernArt", // Too long
            "1.0"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsBadRequest_WhenVersionFormatIsInvalid()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "ModernArt",
            "invalid-version"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid version format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsBadRequest_WhenAllFieldsAreEmpty()
    {
        // Arrange
        var invalidRequest = new AddExampleLinkRequest(
            string.Empty,
            string.Empty,
            string.Empty
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "All fields are required");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsConflict_WhenLinkAlreadyExists()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/existing-image.jpg",
            "ModernArt",
            "1.0"
        );

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status409Conflict,
            "Link already exists");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsConflict_WhenStyleDoesNotExist()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "NonExistentStyle",
            "1.0"
        );

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status409Conflict,
            "Style 'NonExistentStyle' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsConflict_WhenVersionDoesNotExist()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "ModernArt",
            "99.0"
        );

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status409Conflict,
            "Version '99.0' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsConflict_WhenBothStyleAndVersionDoNotExist()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "NonExistentStyle",
            "99.0"
        );

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status409Conflict,
            "Style and version not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsInternalServerError_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "ModernArt",
            "1.0"
        );

        var failureResult = CreateFailureResult<string, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task AddExampleLink_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/test-image.jpg",
            "TestStyle",
            "2.0"
        );

        var result = Result.Ok("test-link-id");
        var senderMock = new Mock<ISender>();
        AddExampleLink.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<string>>, CancellationToken>((cmd, ct) =>
            {
                capturedCommand = cmd as AddExampleLink.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(request.Link, capturedCommand!.Link);
        Assert.Equal(request.Style, capturedCommand.StyleName);
        Assert.Equal(request.Version, capturedCommand.Version);
    }

    [Fact]
    public async Task AddExampleLink_HandlesCancellationToken()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "ModernArt",
            "1.0"
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.AddExampleLink(request, cts.Token));
    }

    [Theory]
    [InlineData("http://example.com/image1.jpg", "Style1", "1.0")]
    [InlineData("https://example.com/image2.png", "Style2", "2.0")]
    [InlineData("http://test.com/test.jpeg", "TestStyle", "5.2")]
    public async Task AddExampleLink_ReturnsCreated_ForVariousValidInputs(
        string link, string style, string version)
    {
        // Arrange
        var request = new AddExampleLinkRequest(link, style, version);
        var result = Result.Ok($"generated-id-{Guid.NewGuid()}");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertCreatedResult<string>(actionResult, nameof(ExampleLinksController.CheckLinkExists));
    }

    [Theory]
    [InlineData("", "Style", "1.0")]
    [InlineData("http://example.com", "", "1.0")]
    [InlineData("http://example.com", "Style", "")]
    [InlineData(null, "Style", "1.0")]
    [InlineData("http://example.com", null, "1.0")]
    [InlineData("http://example.com", "Style", null)]
    public async Task AddExampleLink_ReturnsBadRequest_ForInvalidInputCombinations(
        string? link, string? style, string? version)
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            link ?? string.Empty,
            style ?? string.Empty,
            version ?? string.Empty
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid input data");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}