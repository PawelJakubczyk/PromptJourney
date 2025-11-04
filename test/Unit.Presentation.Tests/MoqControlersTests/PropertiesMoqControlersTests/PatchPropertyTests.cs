using Application.UseCases.Properties.Commands;
using Application.UseCases.Properties.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests;

public sealed class PatchPropertyTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task PatchProperty_ReturnsOkWithCommandResponse_WhenPropertyPatchedSuccessfully()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "DefaultValue",
            "2:1"
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyCommandResponse>(actionResult);
    }

    [Fact]
    public async Task PatchProperty_ReturnsNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var propertyName = "nonexistent";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "DefaultValue",
            "2:1"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Property '{propertyName}' not found for version '{version}'");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenCharacteristicIsEmpty()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var invalidRequest = new PatchPropertyRequest(
            propertyName,
            version,
            string.Empty, // Invalid characteristic
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Characteristic cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenCharacteristicNotSupported()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "UnsupportedCharacteristic",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Characteristic 'UnsupportedCharacteristic' is not supported for patching");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenVersionIsEmpty()
    {
        // Arrange
        var propertyName = "aspect";
        var emptyVersion = string.Empty;
        var request = new PatchPropertyRequest(
            propertyName,
            emptyVersion,
            "DefaultValue",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenPropertyNameIsEmpty()
    {
        // Arrange
        var emptyPropertyName = string.Empty;
        var version = "1.0";
        var request = new PatchPropertyRequest(
            emptyPropertyName,
            version,
            "DefaultValue",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenNewValueIsEmpty()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "DefaultValue",
            string.Empty // Empty new value
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "New value cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var propertyName = "aspect";
        var whitespaceVersion = "   ";
        var request = new PatchPropertyRequest(
            propertyName,
            whitespaceVersion,
            "DefaultValue",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenPropertyNameIsWhitespace()
    {
        // Arrange
        var whitespacePropertyName = "   ";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            whitespacePropertyName,
            version,
            "DefaultValue",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenCharacteristicIsWhitespace()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "   ", // Whitespace characteristic
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Characteristic cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenVersionFormatIsInvalid()
    {
        // Arrange
        var propertyName = "aspect";
        var invalidVersion = "invalid-version";
        var request = new PatchPropertyRequest(
            propertyName,
            invalidVersion,
            "DefaultValue",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid version format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var propertyName = "aspect";
        var nonExistentVersion = "99.0";
        var request = new PatchPropertyRequest(
            propertyName,
            nonExistentVersion,
            "DefaultValue",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Version '{nonExistentVersion}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "DefaultValue",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var propertyName = "chaos";
        var version = "2.0";
        var characteristic = "MaxValue";
        var newValue = "100";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            characteristic,
            newValue
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        PatchProperty.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<PropertyCommandResponse>>, CancellationToken>((cmd, ct) =>
            {
                capturedCommand = cmd as PatchProperty.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(version, capturedCommand!.Version);
        Assert.Equal(propertyName, capturedCommand.PropertyName);
        Assert.Equal(characteristic, capturedCommand.CharacteristicToUpdate);
        Assert.Equal(newValue, capturedCommand.NewValue);
    }

    [Fact]
    public async Task PatchProperty_HandlesCancellationToken()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "DefaultValue",
            "value"
        );
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.PatchProperty(request, cts.Token));
    }

    [Fact]
    public async Task PatchProperty_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "DefaultValue",
            "value"
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("aspect", "1.0", "DefaultValue", "16:9")]
    [InlineData("chaos", "2.0", "MaxValue", "100")]
    [InlineData("style", "5.2", "MinValue", "0")]
    [InlineData("weird", "6.0", "Description", "New description")]
    public async Task PatchProperty_ReturnsOk_ForVariousValidInputs(string propertyName, string version, string characteristic, string newValue)
    {
        // Arrange
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            characteristic,
            newValue
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyCommandResponse>(actionResult);
    }

    [Fact]
    public async Task PatchProperty_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "DefaultValue",
            "16:9"
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.PatchProperty(request, CancellationToken.None);
        var actionResult2 = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<PropertyCommandResponse>(actionResult1);
        AssertOkResult<PropertyCommandResponse>(actionResult2);
    }

    [Theory]
    [InlineData("DefaultValue")]
    [InlineData("MinValue")]
    [InlineData("MaxValue")]
    [InlineData("Description")]
    public async Task PatchProperty_ReturnsOk_ForAllSupportedCharacteristics(string characteristic)
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            characteristic,
            "new-value"
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyCommandResponse>(actionResult);
    }

    [Fact]
    public async Task PatchProperty_ReturnsOk_WithSpecialCharactersInNewValue()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "Description",
            "New description with special chars: @#$% & symbols"
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyCommandResponse>(actionResult);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "DefaultValue",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during patching");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "DefaultValue",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var propertyName = "aspect";
        var version = "1.0";
        var request = new PatchPropertyRequest(
            propertyName,
            version,
            "DefaultValue",
            "16:9"
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<PatchProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}