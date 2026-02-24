using Application.UseCases.Versions.Queries;
using Application.UseCases.Versions.Responses;
using Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Globalization;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Results;

namespace Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests;

public sealed class GetByVersionTests : VersionsControllerTestsBase
{
    [Theory]
    [InlineData("1.0", "--v 1.0", "2025-12-22", "Version 1.0")]
    [InlineData("5.1", "--v 5.1", "2025-12-22", "Version 5.1")]
    [InlineData("niji 6", "--niji 6", "2025-12-22", "Niji version 6")]
    [InlineData("niji 3", "--niji 3", null, null)]
    [InlineData("7.1", "--v 7.1", "2025-12-22", null)]
    [InlineData("5.1", "--v 5.1", null, "Version 5.1")]
    [InlineData("niji 6", "--niji 6", null, "Niji version 6")]
    public async Task GetByVersion_ReturnsOk_WhenVersionExists
    (
        string version,
        string command,
        string? releaseDate,
        string? description
    )
    {
        // Arrange
        DateTime? releaseDateTime = releaseDate != null
            ? DateTime.ParseExact(releaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            : null;

        var versionResponse = new VersionResponse(version, command, releaseDateTime, description);

        var result = Result.Ok(versionResponse);

        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetVersion.Query, VersionResponse>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(versionResponse);
    }


    [Theory]
    [InlineData("99.0")]
    [InlineData("niji 17")]
    [InlineData("13")]
    public async Task GetByVersion_ReturnsNotFound_WhenVersionDoesNotExist(string version)
    {
        // Arrange
        var noFoundMessage = ErrorsMessages.NotFoundMessage(version);

        var failureResult = CreateFailureResult<VersionResponse, ApplicationLayer>
        (
            StatusCodes.Status404NotFound,
            noFoundMessage
        );

        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetVersion.Query, VersionResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeNotFoundResult()
            .WithMessage(noFoundMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task GetByVersion_ReturnsBadRequest_WhenVersionIsNullOrWhitspace(string? invalidVersion)
    {
        // Arrange
        var nullOrWhitespaceVersionMessage = ErrorsMessages.NullOrWhitespaceMessage<ModelVersion>();

        var failureResult = CreateFailureResult<VersionResponse, DomainLayer>
        (
            StatusCodes.Status400BadRequest,
            nullOrWhitespaceVersionMessage
        );

        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetVersion.Query, VersionResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(invalidVersion, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage(nullOrWhitespaceVersionMessage);
    }

    [Theory]
    [InlineData("0")]               // not allowed - must start from 1-9
    [InlineData("01")]              // leading zero invalid
    [InlineData("5.10")]            // too many decimal digits
    [InlineData("13.13.13.13")]     // invalid pattern
    [InlineData("5.")]              // incomplete decimal
    [InlineData(".5")]              // invalid
    [InlineData("wrong format")]    // text
    [InlineData("niji")]            // incomplete niji
    [InlineData("niji5")]           // missing space
    [InlineData("niji 0")]          // zero invalid
    [InlineData("niji -5")]         // negative invalid
    [InlineData("niji 5.1")]        // decimal not allowed for niji
    [InlineData("9999")]            // to many digits
    public async Task GetByVersion_ReturnsBadRequest_WhenVersionFormatIsInvalid(string invalidVersion)
    {
        // Arrange
        var invalidFormatVersionMessage = ModelVersionErrorsExtensions.InvalidVersionFormatMessage;

        var failureResult = CreateFailureResult<VersionResponse, DomainLayer>
        (
            StatusCodes.Status400BadRequest,
            invalidFormatVersionMessage
        );

        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetVersion.Query, VersionResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(invalidVersion, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage(invalidFormatVersionMessage);
    }

    [Fact]
    public async Task GetByVersion_ReturnsBadRequest_WhenVersionExceedsMaxLength()
    {
        // Arrange
        var tooLongVersion = new string('1', 256);
        var tooLongVersionMessage = ErrorsMessages.TooLongMessage<ModelVersion>(tooLongVersion, ModelVersion.MaxLength);

        var failureResult = CreateFailureResult<VersionResponse, DomainLayer>
        (
            StatusCodes.Status400BadRequest,
            tooLongVersionMessage
        );

        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetVersion.Query, VersionResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(tooLongVersion, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage(tooLongVersionMessage);
    }

    [Fact]
    public async Task GetByVersion_HandlesCancellationToken()
    {
        // Arrange
        var version = "1.0";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions
            .Awaiting(() => controller.GetByVersion(version, cts.Token))
            .Should()
            .ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetByVersion_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var version = "1.0";
        var versionResponse = new VersionResponse(version, "--v 1.0", DateTime.UtcNow, "Version 1.0");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByVersion_ReturnsConsistentResults_ForSameVersion()
    {
        // Arrange
        var version = "1.0";
        var versionResponse = new VersionResponse(version, "--v 1.0", DateTime.UtcNow, "Version 1.0");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetByVersion(version, CancellationToken.None);
        var actionResult2 = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult1
            .Should()
            .BeOkResult()
            .WithValue(versionResponse);
        actionResult2
            .Should()
            .BeOkResult()
            .WithValue(versionResponse);
    }
    /// <summary>
    /// ///////////////////////////////////////////////////////
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetByVersion_ReturnsBadRequest_WhenRepositoryFails()
    {
        // Arrange
        var version = "1.0";
        var failureResult = CreateFailureResult<VersionResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Repository error during version retrieval");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Repository error during version retrieval");
    }
}