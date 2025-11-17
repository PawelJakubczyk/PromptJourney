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

public sealed class GetByStyleTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task GetByStyle_ReturnsOkWithList_WhenStyleExists()
    {
        // Arrange
        var list = new List<ExampleLinkResponse> 
        { 
            new(CorrectUrl, CorrectStyleName, CorrectVersion), 
            new("http://example2.com/image2.png", CorrectStyleName, "2.0") 
        };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetByStyle_ReturnsOkWithEmptyList_WhenStyleHasNoLinks()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(
            Result.Ok(new List<ExampleLinkResponse>()));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle("EmptyStyle", CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(0);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            "Style name cannot be empty");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(string.Empty, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound, 
            $"Style '{NonExistStyleName}' not found");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(NonExistStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            "Style name cannot be whitespace");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle("   ", CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            ErrorMessageStyleNameTooLong);
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(IncorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            "Style name cannot be null");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(null!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError, 
            "Database connection failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, CorrectStyleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        GetExampleLinksByStyle.Query? captured = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<ExampleLinkResponse>>>, CancellationToken>((q, ct) => { captured = q as GetExampleLinksByStyle.Query; })
            .ReturnsAsync(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        await controller.GetByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.StyleName.Should().Be(CorrectStyleName);
    }

    [Fact]
    public async Task GetByStyle_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendThrowsOperationCanceledForAny<List<ExampleLinkResponse>>();
        var controller = CreateController(senderMock);

        // Act
        var action = () => controller.GetByStyle(CorrectStyleName, cts.Token);

        // Assert
        await action.Should().ThrowAsync<OperationCanceledException>()
            .WithMessage(ErrorCanceledOperation);
    }

    [Fact]
    public async Task GetByStyle_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(
            Result.Ok(new List<ExampleLinkResponse>()));
        var controller = CreateController(senderMock);

        // Act
        await controller.GetByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Theory]
    [InlineData("ModernArt", 5)]
    [InlineData("ClassicStyle", 3)]
    [InlineData("Abstract", 1)]
    [InlineData("Minimal", 0)]
    public async Task GetByStyle_ReturnsOk_ForVariousStylesAndCounts(string styleName, int count)
    {
        // Arrange
        var list = Enumerable.Range(1, count)
            .Select(i => new ExampleLinkResponse($"http://example{i}.com/image.jpg", styleName, $"{i % 6}.0"))
            .ToList();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(count);
    }

    [Fact]
    public async Task GetByStyle_ReturnsConsistentResults_ForSameStyleName()
    {
        // Arrange
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, CorrectStyleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var r1 = await controller.GetByStyle(CorrectStyleName, CancellationToken.None);
        var r2 = await controller.GetByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        r1.Should().BeOkResult().WithCount(1);
        r2.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByStyle_ReturnsOk_WithLinksFromDifferentVersions()
    {
        // Arrange
        var list = new List<ExampleLinkResponse> 
        { 
            new("http://example1.com/image1.jpg", CorrectStyleName, "1.0"), 
            new("http://example2.com/image2.jpg", CorrectStyleName, "2.0"), 
            new("http://example3.com/image3.jpg", CorrectStyleName, "5.2"), 
            new("http://example4.com/image4.jpg", CorrectStyleName, "6.0") 
        };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetByStyle_HandlesSpecialCharactersInStyleName()
    {
        // Arrange
        var style = "Modern-Art_2024";
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, style, CorrectVersion) };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(style, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Theory]
    [InlineData("ModernArt")]
    [InlineData("ClassicStyle")]
    [InlineData("Abstract")]
    [InlineData("Minimal")]
    [InlineData("Vintage")]
    public async Task GetByStyle_ReturnsOk_ForVariousValidStyleNames(string styleName)
    {
        // Arrange
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, styleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task GetByStyle_ReturnsBadRequest_ForInvalidStyleNames(string invalidStyleName)
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            "Invalid style name");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(invalidStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest, 
            "Repository error");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failure = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest, 
            "Query handler failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_HandlesCaseInsensitiveStyleNames()
    {
        // Arrange
        var styleName = "modernart";
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, styleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByStyle_ReturnsOk_WithLargeNumberOfLinks()
    {
        // Arrange
        var styleName = "PopularStyle";
        var largeList = Enumerable.Range(1, 100)
            .Select(i => new ExampleLinkResponse($"http://example{i}.com/image{i}.jpg", styleName, $"{i % 6}.0"))
            .ToList();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(Result.Ok(largeList));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(100);
    }

    [Fact]
    public async Task GetByStyle_ReturnsOk_WithMixedUrlFormats()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", CorrectStyleName, CorrectVersion),
            new("https://secure.example.com/image.png", CorrectStyleName, CorrectVersion),
            new("http://example.com/path/to/image.jpeg", CorrectStyleName, CorrectVersion)
        };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetByStyle_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetExampleLinksByStyle.Query, List<ExampleLinkResponse>>(
            Result.Ok(new List<ExampleLinkResponse>()));
        var controller = CreateController(senderMock);
        var start = DateTime.UtcNow;

        // Act
        await controller.GetByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        (DateTime.UtcNow - start).Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}