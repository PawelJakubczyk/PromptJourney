using Application.UseCases.ExampleLinks.Queries;
using Application.UseCases.ExampleLinks.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class GetByStyleAndVersionTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task GetByStyleAndVersion_ReturnsOkWithList_WhenStyleAndVersionExist()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>
        {
            new(CorrectUrl, CorrectStyleName, CorrectVersion),
            new("http://example2.com/image2.png", CorrectStyleName, CorrectVersion)
        };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsOkWithEmptyList_WhenNoMatchingLinks()
    {
        // Arrange
        var emptyList = new List<ExampleLinkResponse>();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(Result.Ok(emptyList));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(CorrectStyleName, NonExistVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(0);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenStyleNameIsInvalid()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(string.Empty, CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenVersionIsInvalid()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(CorrectStyleName, string.Empty, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenBothParametersAreInvalid()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name and version cannot be empty");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(string.Empty, string.Empty, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Style '{NonExistStyleName}' not found");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(NonExistStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            ErrorMessageVersionNotFound);
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(CorrectStyleName, NonExistVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsNotFound_WhenBothStyleAndVersionDoNotExist()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            ErrorMessageStyleAndVersionNotFound);
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(NonExistStyleName, NonExistVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion("   ", CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(CorrectStyleName, "   ", CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            ErrorMessageStyleNameTooLong);
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(IncorrectStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, CorrectStyleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        GetExampleLinksByStyleAndVersion.Query? captured = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<ExampleLinkResponse>>>, CancellationToken>((q, ct) => { captured = q as GetExampleLinksByStyleAndVersion.Query; })
            .ReturnsAsync(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        await controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.StyleName.Should().Be(CorrectStyleName);
        captured.Version.Should().Be(CorrectVersion);
    }

    [Fact]
    public async Task GetByStyleAndVersion_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendThrowsOperationCanceledForAny<List<ExampleLinkResponse>>();
        var controller = CreateController(senderMock);

        // Act
        var action = () => controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, cts.Token);

        // Assert
        await action.Should().ThrowAsync<OperationCanceledException>()
            .WithMessage(ErrorCanceledOperation);
    }

    [Fact]
    public async Task GetByStyleAndVersion_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(
            Result.Ok(new List<ExampleLinkResponse>()));
        var controller = CreateController(senderMock);

        // Act
        await controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernArt", "1.0", 3)]
    [InlineData("ClassicStyle", "2.0", 5)]
    [InlineData("Abstract", "5.2", 1)]
    [InlineData("Minimal", "6.0", 0)]
    public async Task GetByStyleAndVersion_ReturnsOk_ForVariousCombinations(string styleName, string version, int count)
    {
        // Arrange
        var list = Enumerable.Range(1, count)
            .Select(i => new ExampleLinkResponse($"http://example{i}.com/image.jpg", styleName, version))
            .ToList();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(count);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, CorrectStyleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var r1 = await controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, CancellationToken.None);
        var r2 = await controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        r1.Should().BeOkResult().WithCount(1);
        r2.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByStyleAndVersion_HandlesSpecialCharactersInStyleName()
    {
        // Arrange
        var styleName = "Modern-Art_2024";
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, styleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Theory]
    [InlineData("ModernArt", "1.0")]
    [InlineData("ClassicStyle", "2.0")]
    [InlineData("Abstract", "5.2")]
    [InlineData("Minimal", "6.0")]
    [InlineData("niji", "5")]
    public async Task GetByStyleAndVersion_ReturnsOk_ForVariousValidInputs(string styleName, string version)
    {
        // Arrange
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, styleName, version) };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_HandlesCaseInsensitiveStyleNames()
    {
        // Arrange
        var styleName = "modernart";
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, styleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsOk_WithMultipleLinksForSameStyleAndVersion()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com/image1.jpg", CorrectStyleName, CorrectVersion),
            new("http://example2.com/image2.png", CorrectStyleName, CorrectVersion),
            new("http://example3.com/image3.jpeg", CorrectStyleName, CorrectVersion),
            new("http://example4.com/image4.webp", CorrectStyleName, CorrectVersion)
        };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetByStyleAndVersion_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyleAndVersion.Query, List<ExampleLinkResponse>>(
            Result.Ok(new List<ExampleLinkResponse>()));
        var controller = CreateController(senderMock);
        var start = DateTime.UtcNow;

        // Act
        await controller.GetByStyleAndVersion(CorrectStyleName, CorrectVersion, CancellationToken.None);

        // Assert
        (DateTime.UtcNow - start).Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}