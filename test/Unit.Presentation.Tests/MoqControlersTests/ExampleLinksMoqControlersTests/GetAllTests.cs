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

public sealed class GetAllTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsOkWithList_WhenLinksExist()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>
        {
            new(CorrectUrl, CorrectStyleName, CorrectVersion),
            new("http://example2.com/image2.png", "ClassicStyle", "2.0")
        };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithEmptyList_WhenNoLinksExist()
    {
        // Arrange
        var emptyList = new List<ExampleLinkResponse>();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(emptyList));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(0);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_UsesSingletonQuery()
    {
        // Arrange
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, CorrectStyleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        GetAllExampleLinks.Query? capturedQuery = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllExampleLinks.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<ExampleLinkResponse>>>, CancellationToken>((query, ct) => { capturedQuery = query as GetAllExampleLinks.Query; })
            .ReturnsAsync(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery.Should().BeSameAs(GetAllExampleLinks.Query.Singletone);
    }

    [Fact]
    public async Task GetAll_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendThrowsOperationCanceledForAny<List<ExampleLinkResponse>>();
        var controller = CreateController(senderMock);

        // Act
        var action = () => controller.GetAll(cts.Token);

        // Assert
        await action.Should().ThrowAsync<OperationCanceledException>()
            .WithMessage(ErrorCanceledOperation);
    }

    [Fact]
    public async Task GetAll_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(It.IsAny<GetAllExampleLinks.Query>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsConsistentResults_WhenCalledMultipleTimes()
    {
        // Arrange
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, CorrectStyleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var r1 = await controller.GetAll(CancellationToken.None);
        var r2 = await controller.GetAll(CancellationToken.None);

        // Assert
        r1.Should().BeOkResult().WithCount(1);
        r2.Should().BeOkResult().WithCount(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task GetAll_ReturnsOk_ForVariousListSizes(int count)
    {
        // Arrange
        var list = Enumerable.Range(1, count)
            .Select(i => new ExampleLinkResponse($"http://example{i}.com/image.jpg", $"Style{i}", "1.0"))
            .ToList();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(count);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithDifferentStylesAndVersions()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com/image1.jpg", CorrectStyleName, CorrectVersion),
            new("http://example2.com/image2.jpg", "ClassicStyle", "2.0"),
            new("http://example3.com/image3.jpg", "Abstract", "5.2"),
            new("http://example4.com/image4.jpg", "Minimal", "6.0")
        };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_DoesNotRequireParameters()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(0);
    }

    [Fact]
    public async Task GetAll_UsesSingletonPattern_VerifiesSameInstance()
    {
        // Arrange
        var list = new List<ExampleLinkResponse> { new(CorrectUrl, CorrectStyleName, CorrectVersion) };
        var senderMock = CreateSenderMock();
        var capturedQueries = new List<GetAllExampleLinks.Query>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllExampleLinks.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<ExampleLinkResponse>>>, CancellationToken>((query, ct) => { if (query is GetAllExampleLinks.Query q) capturedQueries.Add(q); })
            .ReturnsAsync(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);
        await controller.GetAll(CancellationToken.None);
        await controller.GetAll(CancellationToken.None);

        // Assert
        capturedQueries.Should().HaveCount(3);
        capturedQueries.Should().AllSatisfy(q => q.Should().BeSameAs(GetAllExampleLinks.Query.Singletone));
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithMixedUrlFormats()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", "Style1", CorrectVersion),
            new("https://secure.example.com/image.png", "Style2", CorrectVersion),
            new("http://example.com/path/to/image.jpeg", "Style3", CorrectVersion)
        };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetAll_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);
        var start = DateTime.UtcNow;

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        (DateTime.UtcNow - start).Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithLargeDataset()
    {
        // Arrange
        var largeList = Enumerable.Range(1, 1000)
            .Select(i => new ExampleLinkResponse($"http://example{i}.com/image{i}.jpg", $"Style{i % 10}", $"{i % 6}.0"))
            .ToList();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(largeList));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(1000);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenAllLinksHaveSameStyle()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com/image1.jpg", CorrectStyleName, CorrectVersion),
            new("http://example2.com/image2.jpg", CorrectStyleName, CorrectVersion),
            new("http://example3.com/image3.jpg", CorrectStyleName, CorrectVersion)
        };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenAllLinksHaveSameVersion()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com/image1.jpg", "Style1", "5.2"),
            new("http://example2.com/image2.jpg", "Style2", "5.2"),
            new("http://example3.com/image3.jpg", "Style3", "5.2")
        };
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<GetAllExampleLinks.Query, List<ExampleLinkResponse>>(Result.Ok(list));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(3);
    }
}