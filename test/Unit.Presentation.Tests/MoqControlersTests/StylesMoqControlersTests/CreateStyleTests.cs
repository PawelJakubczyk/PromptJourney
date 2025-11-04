using Application.UseCases.Styles.Commands;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class CreateStyleTests : StylesControllerTestsBase
{
    [Fact]
    public async Task Create_ReturnsCreatedWithStyleName_WhenStyleCreatedSuccessfully()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            "A new custom style",
            ["modern", "creative"]
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(StylesController.GetByName));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            string.Empty,
            "Custom"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTypeIsEmpty()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            "NewStyle",
            string.Empty
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenBothNameAndTypeAreEmpty()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            string.Empty,
            string.Empty
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name and type cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenStyleAlreadyExists()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "ExistingStyle",
            "Custom"
        );

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status409Conflict,
            $"Style '{request.Name}' already exists");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsWhitespace()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            "   ",
            "Custom"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTypeIsWhitespace()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            "NewStyle",
            "   "
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            null!,
            "Custom"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTypeIsNull()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            "NewStyle",
            null!
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameExceedsMaxLength()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            new string('a', 256),
            "Custom"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTypeExceedsMaxLength()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            "NewStyle",
            new string('a', 256)
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            new string('a', 1000)
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Description exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTagIsEmpty()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            "Description",
            [string.Empty]
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTagExceedsMaxLength()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            "Description",
            [new string('a', 256)]
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom"
        );

        var failureResult = CreateFailureResult<string, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        // ToResultsCreatedAsync maps all non-409/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "CustomStyle",
            "Modern",
            "A modern custom style",
            ["creative", "minimalist"]
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        AddStyle.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<string>>, CancellationToken>((cmd, ct) =>
            {
                capturedCommand = cmd as AddStyle.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.Create(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(request.Name, capturedCommand!.Name);
        Assert.Equal(request.Type, capturedCommand.Type);
        Assert.Equal(request.Description, capturedCommand.Description);
        Assert.Equal(request.Tags, capturedCommand.Tags);
    }

    [Fact]
    public async Task Create_HandlesCancellationToken()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom"
        );
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.Create(request, cts.Token));
    }

    [Fact]
    public async Task Create_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom"
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.Create(request, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernArt", "Custom")]
    [InlineData("ClassicStyle", "Preset")]
    [InlineData("AbstractDesign", "Modern")]
    [InlineData("MinimalStyle", "Artistic")]
    public async Task Create_ReturnsCreated_ForVariousValidInputs(string name, string type)
    {
        // Arrange
        var request = new CreateStyleRequest(name, type);
        var result = Result.Ok(name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(StylesController.GetByName));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithNullDescription()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            null
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(StylesController.GetByName));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithEmptyTagsList()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            "Description",
            []
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(StylesController.GetByName));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithNullTagsList()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            "Description",
            null
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(StylesController.GetByName));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithMultipleTags()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            "Description",
            ["modern", "creative", "minimalist", "abstract"]
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(StylesController.GetByName));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithLongDescription()
    {
        // Arrange
        var longDescription = new string('a', 500);
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            longDescription
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(StylesController.GetByName));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithSpecialCharactersInName()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "Modern-Art_Style",
            "Custom"
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(StylesController.GetByName));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithSpecialCharactersInDescription()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            "A style with special chars: @#$% & symbols (test)"
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(StylesController.GetByName));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom"
        );

        var failureResult = CreateFailureResult<string, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during style creation");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom"
        );

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom"
        );

        var result = Result.Ok(request.Name);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.Create(request, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}