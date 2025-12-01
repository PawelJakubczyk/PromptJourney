using Application.UseCases.ExampleLinks.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using FluentAssertions;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class AddExampleLinkTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task AddExampleLink_ReturnsCreated_WhenLinkAddedSuccessfully()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(resultOk);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(requestOk, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeCreatedResult()
            .WithValue(CorrectId);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsBadRequest_WhenLinkFormatIsInvalid()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(failureInvalidLinkFormat);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(requestInvalidUrl, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage(ErrorMessageInvalidLinkFormat);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsBadRequest_WhenStyleNameIsInvalid()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(failureStyleNameTooLong);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(requestInvalidStyleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage(ErrorMessageStyleNameTooLong);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsBadRequest_WhenVersionFormatIsInvalid()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(failureInvalidVersionFormat);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(requestInvalidVersion, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage(ErrorMessageInvalidVersionFormat);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsBadRequest_WhenAllFieldsAreEmpty()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(failureAllFieldsRequired);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(requestAllEmpty, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage(ErrorMessageAllFieldsRequired);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsConflict_WhenLinkAlreadyExists()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(failureLinkAlreadyExists);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(requestExistingLink, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeConflictResult()
            .WithMessage(ErrorMessageLinkAlreadyExists);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsConflict_WhenStyleDoesNotExist()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(failureStyleNotFound);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(requestNonExistentStyle, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeConflictResult()
            .WithMessage(ErrorMessageStyleNotFound);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(failureVersionNotFound);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(requestNonExistentVersion, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeNotFoundResult()
            .WithMessage(ErrorMessageVersionNotFound);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsNotFound_WhenBothStyleAndVersionDoNotExist()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(failureStyleAndVersionNotFound);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(requestNonExistentBoth, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeNotFoundResult()
            .WithMessage(ErrorMessageStyleAndVersionNotFound);
    }

    [Fact]
    public async Task AddExampleLink_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendThrowsOperationCanceledForAny<string>();
        var controller = CreateController(senderMock);

        // Act 
        var actiom = () => controller.AddExampleLink(requestOk, cts.Token);

        // Assert
        await actiom
            .Should()
            .ThrowAsync<OperationCanceledException>()
            .WithMessage(ErrorCanceledOperation);
    }

    [Theory]
    [InlineData("http://example.com/image1.jpg", "Style1", "1.0")]
    [InlineData("https://example.com/image2.png", "Style2", "2.0")]
    [InlineData("http://test.com/test.jpeg", "TestStyle", "5.2")]
    public async Task AddExampleLink_ReturnsCreated_ForVariousValidInputs(string link, string style, string version)
    {
        // Arrange
        var request = new AddExampleLinkRequest(link, style, version);
        var id = Guid.NewGuid().ToString();
        var result = Result.Ok(id);
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeCreatedResult()
            .WithValue(id);
    }

    [Theory]
    [InlineData("", "Style", "1.0")]
    [InlineData("http://example.com", "", "1.0")]
    [InlineData("http://example.com", "Style", "")]
    [InlineData(null, "Style", "1.0")]
    [InlineData("http://example.com", null, "1.0")]
        [InlineData("http://example.com", "Style", null)]
    public async Task AddExampleLink_ReturnsBadRequest_ForInvalidInputCombinations(string? link, string? style, string? version)
    {
        // Arrange
        var request = new AddExampleLinkRequest(link ?? string.Empty, style ?? string.Empty, version ?? string.Empty);
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<AddExampleLink.Command, string>(failureInvalidInputData);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage(ErrorMessageInvalidInputData);
    }
}