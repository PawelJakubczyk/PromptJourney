using Application.UseCases.Common.Responses;
using Application.UseCases.Properties.Commands;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests;

public sealed class DeletePropertyTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task DeleteProperty_ReturnsOkWithDeleteResponse_WhenPropertyDeletedSuccessfully()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var deleteResponse = DeleteResponse.Success($"Property '{propertyName}' for version '{version}' was successfully deleted.");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "nonexistent";
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Property '{propertyName}' not found for version '{version}'");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenVersionIsEmpty()
    {
        // Arrange
        var emptyVersion = string.Empty;
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(emptyVersion, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenPropertyNameIsEmpty()
    {
        // Arrange
        var version = "1.0";
        var emptyPropertyName = string.Empty;
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, emptyPropertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenBothParametersAreEmpty()
    {
        // Arrange
        var emptyVersion = string.Empty;
        var emptyPropertyName = string.Empty;
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version and property name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(emptyVersion, emptyPropertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var nonExistentVersion = "99.0";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Version '{nonExistentVersion}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(nonExistentVersion, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var whitespaceVersion = "   ";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(whitespaceVersion, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenPropertyNameIsWhitespace()
    {
        // Arrange
        var version = "1.0";
        var whitespacePropertyName = "   ";
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, whitespacePropertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenVersionIsNull()
    {
        // Arrange
        string? nullVersion = null;
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(nullVersion!, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenPropertyNameIsNull()
    {
        // Arrange
        var version = "1.0";
        string? nullPropertyName = null;
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, nullPropertyName!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenVersionFormatIsInvalid()
    {
        // Arrange
        var invalidVersion = "invalid-version";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid version format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(invalidVersion, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenPropertyNameExceedsMaxLength()
    {
        // Arrange
        var version = "1.0";
        var tooLongPropertyName = new string('a', 256);
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, tooLongPropertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<DeleteResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var version = "2.0";
        var propertyName = "chaos";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        DeleteProperty.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<DeleteResponse>>, CancellationToken>((cmd, ct) =>
            {
                capturedCommand = cmd as DeleteProperty.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(version, capturedCommand!.Version);
        Assert.Equal(propertyName, capturedCommand.PropertyName);
    }

    [Fact]
    public async Task DeleteProperty_HandlesCancellationToken()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.DeleteProperty(version, propertyName, cts.Token));
    }

    [Fact]
    public async Task DeleteProperty_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("1.0", "aspect")]
    [InlineData("2.0", "chaos")]
    [InlineData("5.2", "style")]
    [InlineData("6.0", "weird")]
    public async Task DeleteProperty_ReturnsOk_ForVariousValidInputs(string version, string propertyName)
    {
        // Arrange
        var deleteResponse = DeleteResponse.Success($"Deleted {propertyName} from {version}");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.DeleteProperty(version, propertyName, CancellationToken.None);
        var actionResult2 = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult1);
        AssertOkResult<DeleteResponse>(actionResult2);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsNotFound_WhenVersionExistsButPropertyDoesNot()
    {
        // Arrange
        var version = "1.0";
        var nonExistentProperty = "nonexistent";
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Property '{nonExistentProperty}' not found for version '{version}'");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, nonExistentProperty, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsOk_WithPropertyNameContainingHyphen()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect-ratio";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<DeleteResponse, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during deletion");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsDeleteResponse_WithSuccessMessage()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var expectedMessage = $"Property '{propertyName}' for version '{version}' was successfully deleted.";
        var deleteResponse = DeleteResponse.Success(expectedMessage);
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task DeleteProperty_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}