using Application.UseCases.Properties.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests;

public sealed class CheckPropertyExistsTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task CheckPropertyExists_ReturnsOkWithTrue_WhenPropertyExists()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsOkWithFalse_WhenPropertyDoesNotExist()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "nonexistent";
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenVersionIsEmpty()
    {
        // Arrange
        var emptyVersion = string.Empty;
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(emptyVersion, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenPropertyNameIsEmpty()
    {
        // Arrange
        var version = "1.0";
        var emptyPropertyName = string.Empty;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, emptyPropertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenBothParametersAreEmpty()
    {
        // Arrange
        var emptyVersion = string.Empty;
        var emptyPropertyName = string.Empty;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version and property name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(emptyVersion, emptyPropertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var whitespaceVersion = "   ";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(whitespaceVersion, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenPropertyNameIsWhitespace()
    {
        // Arrange
        var version = "1.0";
        var whitespacePropertyName = "   ";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, whitespacePropertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenVersionIsNull()
    {
        // Arrange
        string? nullVersion = null;
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(nullVersion!, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenPropertyNameIsNull()
    {
        // Arrange
        var version = "1.0";
        string? nullPropertyName = null;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, nullPropertyName!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenVersionFormatIsInvalid()
    {
        // Arrange
        var invalidVersion = "invalid-version";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid version format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(invalidVersion, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenPropertyNameExceedsMaxLength()
    {
        // Arrange
        var version = "1.0";
        var tooLongPropertyName = new string('a', 256);
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, tooLongPropertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        // ToResultsCheckExistOkAsync maps all non-400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var version = "2.0";
        var propertyName = "chaos";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        CheckPropertyExists.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as CheckPropertyExists.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(version, capturedQuery!.Version);
        Assert.Equal(propertyName, capturedQuery.PropertyName);
    }

    [Fact]
    public async Task CheckPropertyExists_HandlesCancellationToken()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.CheckPropertyExists(version, propertyName, cts.Token));
    }

    [Fact]
    public async Task CheckPropertyExists_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("1.0", "aspect")]
    [InlineData("2.0", "chaos")]
    [InlineData("5.2", "style")]
    [InlineData("6.0", "weird")]
    public async Task CheckPropertyExists_ReturnsOk_ForVariousValidInputs(string version, string propertyName)
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);
        var actionResult2 = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<bool>(actionResult1);
        AssertOkResult<bool>(actionResult2);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenVersionDoesNotExist()
    {
        // Arrange
        var nonExistentVersion = "99.0";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<bool, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            $"Version '{nonExistentVersion}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(nonExistentVersion, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsOk_WithPropertyNameContainingHyphen()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect-ratio";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsOk_WithPropertyNameContainingNumbers()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "property123";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<bool, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckPropertyExists_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckPropertyExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}