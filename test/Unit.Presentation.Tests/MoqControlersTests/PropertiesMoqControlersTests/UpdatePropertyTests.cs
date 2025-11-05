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

public sealed class UpdatePropertyTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task UpdateProperty_ReturnsOkWithCommandResponse_WhenPropertyUpdatedSuccessfully()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar", "--aspect"],
            "1:1",
            "1:4",
            "4:1",
            "Updated aspect ratio parameter"
        );

        var response = new PropertyCommandResponse(request.PropertyName, request.Version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<PropertyCommandResponse>();
    }

    [Fact]
    public async Task UpdateProperty_ReturnsNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "nonexistent";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ne"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Property '{propertyName}' not found for version '{version}'");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenParametersIsEmpty()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var invalidRequest = new PropertyRequest(
            version,
            propertyName,
            [] // Empty parameters invalid
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Parameters cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(invalidRequest, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenVersionIsEmpty()
    {
        // Arrange
        var emptyVersion = string.Empty;
        var propertyName = "aspect";
        var request = new PropertyRequest(
            emptyVersion,
            propertyName,
            ["--ar"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenPropertyNameIsEmpty()
    {
        // Arrange
        var version = "1.0";
        var emptyPropertyName = string.Empty;
        var request = new PropertyRequest(
            version,
            emptyPropertyName,
            ["--ar"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var whitespaceVersion = "   ";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            whitespaceVersion,
            propertyName,
            ["--ar"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenPropertyNameIsWhitespace()
    {
        // Arrange
        var version = "1.0";
        var whitespacePropertyName = "   ";
        var request = new PropertyRequest(
            version,
            whitespacePropertyName,
            ["--ar"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Property name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenVersionFormatIsInvalid()
    {
        // Arrange
        var invalidVersion = "invalid-version";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            invalidVersion,
            propertyName,
            ["--ar"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid version format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var nonExistentVersion = "99.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            nonExistentVersion,
            propertyName,
            ["--ar"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Version '{nonExistentVersion}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenParameterFormatIsInvalid()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["invalid-param"] // Parameters should start with --
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid parameter format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var version = "2.0";
        var propertyName = "chaos";
        var parameters = new List<string> { "--chaos", "--c" };
        var defaultValue = "50";
        var minValue = "0";
        var maxValue = "100";
        var description = "Chaos parameter";

        var request = new PropertyRequest(
            version,
            propertyName,
            parameters,
            defaultValue,
            minValue,
            maxValue,
            description
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        UpdateProperty.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<PropertyCommandResponse>>, CancellationToken>((cmd, ct) =>
            {
                capturedCommand = cmd as UpdateProperty.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.Version.Should().Be(version);
        capturedCommand.PropertyName.Should().Be(propertyName);
        capturedCommand.Parameters.Should().Equal(parameters);
        capturedCommand.DefaultValue.Should().Be(defaultValue);
        capturedCommand.MinValue.Should().Be(minValue);
        capturedCommand.MaxValue.Should().Be(maxValue);
        capturedCommand.Description.Should().Be(description);
    }

    [Fact]
    public async Task UpdateProperty_HandlesCancellationToken()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar"]
        );
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions.Awaiting(() => controller.UpdateProperty(request, cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task UpdateProperty_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar"]
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("1.0", "aspect", new[] { "--ar", "--aspect" })]
    [InlineData("2.0", "chaos", new[] { "--chaos", "--c" })]
    [InlineData("5.2", "stylize", new[] { "--s", "--stylize" })]
    [InlineData("6.0", "weird", new[] { "--weird", "--w" })]
    public async Task UpdateProperty_ReturnsOk_ForVariousValidInputs(string version, string propertyName, string[] parameters)
    {
        // Arrange
        var request = new PropertyRequest(
            version,
            propertyName,
            [.. parameters]
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<PropertyCommandResponse>();
    }

    [Fact]
    public async Task UpdateProperty_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar", "--aspect"],
            "16:9",
            "1:4",
            "4:1",
            "Aspect ratio"
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.UpdateProperty(request, CancellationToken.None);
        var actionResult2 = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult1.Should().BeOkResult().WithValueOfType<PropertyCommandResponse>();
        actionResult2.Should().BeOkResult().WithValueOfType<PropertyCommandResponse>();
    }

    [Fact]
    public async Task UpdateProperty_ReturnsOk_WithAllOptionalFieldsNull()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar"],
            null, // DefaultValue
            null, // MinValue
            null, // MaxValue
            null  // Description
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<PropertyCommandResponse>();
    }

    [Fact]
    public async Task UpdateProperty_ReturnsOk_WithAllOptionalFieldsPopulated()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar", "--aspect"],
            "16:9",
            "1:4",
            "4:1",
            "Aspect ratio parameter"
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<PropertyCommandResponse>();
    }

    [Fact]
    public async Task UpdateProperty_ReturnsOk_WithMultipleParameters()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar", "--aspect", "-a", "--ratio"]
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<PropertyCommandResponse>();
    }

    [Fact]
    public async Task UpdateProperty_ReturnsOk_WithSpecialCharactersInDescription()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar"],
            "16:9",
            "1:4",
            "4:1",
            "Description with special chars: @#$% & symbols (test)"
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<PropertyCommandResponse>();
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during update");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateProperty_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PropertyRequest(
            version,
            propertyName,
            ["--ar", "--aspect"]
        );

        var response = new PropertyCommandResponse(propertyName, version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.UpdateProperty(request, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}