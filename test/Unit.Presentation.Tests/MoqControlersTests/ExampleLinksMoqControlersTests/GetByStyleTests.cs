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
        var styleName = "ModernArt";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com/image1.jpg", styleName, "1.0"),
            new("http://example2.com/image2.png", styleName, "2.0")
        };

        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByStyle_ReturnsOkWithEmptyList_WhenStyleHasNoLinks()
    {
        // Arrange
        var styleName = "EmptyStyle";
        var emptyList = new List<ExampleLinkResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var emptyStyleName = string.Empty;
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(emptyStyleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var nonExistentStyleName = "NonExistentStyle";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Style '{nonExistentStyleName}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(nonExistentStyleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var whitespaceStyleName = "   ";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(whitespaceStyleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var tooLongStyleName = new string('A', 256);
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(tooLongStyleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        string? nullStyleName = null;
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(nullStyleName!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var styleName = "ModernArt";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", styleName, "1.0")
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        GetExampleLinksByStyle.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<ExampleLinkResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetExampleLinksByStyle.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(styleName, capturedQuery!.StyleName);
    }

    [Fact]
    public async Task GetByStyle_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "ModernArt";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetByStyle(styleName, cts.Token));
    }

    [Fact]
    public async Task GetByStyle_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "ModernArt";
        var list = new List<ExampleLinkResponse>();
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByStyle(styleName, CancellationToken.None);

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

        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, count);
    }

    [Fact]
    public async Task GetByStyle_ReturnsConsistentResults_ForSameStyleName()
    {
        // Arrange
        var styleName = "ModernArt";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", styleName, "1.0")
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetByStyle(styleName, CancellationToken.None);
        var actionResult2 = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult1, 1);
        AssertOkResult<ExampleLinkResponse>(actionResult2, 1);
    }

    [Fact]
    public async Task GetByStyle_ReturnsOk_WithLinksFromDifferentVersions()
    {
        // Arrange
        var styleName = "ModernArt";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com/image1.jpg", styleName, "1.0"),
            new("http://example2.com/image2.jpg", styleName, "2.0"),
            new("http://example3.com/image3.jpg", styleName, "5.2"),
            new("http://example4.com/image4.jpg", styleName, "6.0")
        };

        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 4);
    }

    [Fact]
    public async Task GetByStyle_HandlesSpecialCharactersInStyleName()
    {
        // Arrange
        var styleNameWithSpecialChars = "Modern-Art_2024";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", styleNameWithSpecialChars, "1.0")
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleNameWithSpecialChars, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 1);
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
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", styleName, "1.0")
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task GetByStyle_ReturnsBadRequest_ForInvalidStyleNames(string invalidStyleName)
    {
        // Arrange
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid style name");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(invalidStyleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "ModernArt";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var styleName = "ModernArt";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_HandlesCaseInsensitiveStyleNames()
    {
        // Arrange
        var lowercaseStyleName = "modernart";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", lowercaseStyleName, "1.0")
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(lowercaseStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByStyle_ReturnsOk_WithLargeNumberOfLinks()
    {
        // Arrange
        var styleName = "PopularStyle";
        var largeList = Enumerable.Range(1, 100)
            .Select(i => new ExampleLinkResponse($"http://example{i}.com/image{i}.jpg", styleName, $"{i % 6}.0"))
            .ToList();

        var result = Result.Ok(largeList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 100);
    }

    [Fact]
    public async Task GetByStyle_ReturnsOk_WithMixedUrlFormats()
    {
        // Arrange
        var styleName = "ModernArt";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", styleName, "1.0"),
            new("https://secure.example.com/image.png", styleName, "1.0"),
            new("http://example.com/path/to/image.jpeg", styleName, "1.0")
        };

        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 3);
    }

    [Fact]
    public async Task GetByStyle_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "ModernArt";
        var list = new List<ExampleLinkResponse>();
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}