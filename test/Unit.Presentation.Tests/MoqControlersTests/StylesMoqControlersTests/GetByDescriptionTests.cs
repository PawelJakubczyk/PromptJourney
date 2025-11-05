using Application.UseCases.Styles.Queries;
using Application.UseCases.Styles.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class GetByDescriptionTests : StylesControllerTestsBase
{
    [Fact]
    public async Task GetByDescription_ReturnsOkWithList_WhenStylesWithKeywordExist()
    {
        // Arrange
        var keyword = "modern";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Modern art style", ["abstract"]),
            new("Style2", "Custom", "Contemporary modern design", ["minimalist"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOkWithEmptyList_WhenNoStylesMatchKeyword()
    {
        // Arrange
        var keyword = "nonexistent";
        var emptyList = new List<StyleResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(0);
    }

    [Fact]
    public async Task GetByDescription_ReturnsBadRequest_WhenKeywordIsEmpty()
    {
        // Arrange
        var emptyKeyword = string.Empty;
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Keyword cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(emptyKeyword, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDescription_ReturnsBadRequest_WhenKeywordIsWhitespace()
    {
        // Arrange
        var whitespaceKeyword = "   ";
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Keyword cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(whitespaceKeyword, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDescription_ReturnsBadRequest_WhenKeywordIsNull()
    {
        // Arrange
        string? nullKeyword = null;
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Keyword cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(nullKeyword!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDescription_ReturnsBadRequest_WhenKeywordExceedsMaxLength()
    {
        // Arrange
        var tooLongKeyword = new string('a', 256);
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Keyword exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(tooLongKeyword, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDescription_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var keyword = "test";
        var failureResult = CreateFailureResult<List<StyleResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDescription_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var keyword = "abstract";
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        GetStylesByDescriptionKeyword.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<StyleResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetStylesByDescriptionKeyword.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(keyword, capturedQuery!.DescriptionKeyword);
    }

    [Fact]
    public async Task GetByDescription_HandlesCancellationToken()
    {
        // Arrange
        var keyword = "test";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetByDescription(keyword, cts.Token));
    }

    [Fact]
    public async Task GetByDescription_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var keyword = "test";
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("modern")]
    [InlineData("abstract")]
    [InlineData("vintage")]
    [InlineData("minimalist")]
    [InlineData("artistic")]
    public async Task GetByDescription_ReturnsOk_ForVariousKeywords(string keyword)
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", $"Description containing {keyword}", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByDescription_ReturnsConsistentResults_ForSameKeyword()
    {
        // Arrange
        var keyword = "modern";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Modern design", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetByDescription(keyword, CancellationToken.None);
        var actionResult2 = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        actionResult1.Should().BeOkResult().WithCount(1);
        actionResult2.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOk_WithCaseInsensitiveKeyword()
    {
        // Arrange
        var lowercaseKeyword = "modern";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "MODERN design style", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(lowercaseKeyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOk_WithPartialKeywordMatch()
    {
        // Arrange
        var keyword = "art";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Modern artistic design", ["tag1"]),
            new("Style2", "Traditional", "Smart art style", ["tag2"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOk_WithMultipleMatchingStyles()
    {
        // Arrange
        var keyword = "modern";
        var styles = Enumerable.Range(1, 10)
            .Select(i => new StyleResponse(
                $"Style{i}",
                "Custom",
                $"Modern design style {i}",
                [$"tag{i}"]))
            .ToList();

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(10);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOk_WithStylesHavingNullTags()
    {
        // Arrange
        var keyword = "modern";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Modern design", null),
            new("Style2", "Traditional", "Modern art", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOk_WithKeywordContainingNumbers()
    {
        // Arrange
        var keyword = "2024";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Design trends for 2024", ["trend"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOk_WithKeywordContainingHyphen()
    {
        // Arrange
        var keyword = "modern-art";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Beautiful modern-art design", ["art"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOk_WithKeywordContainingSpecialCharacters()
    {
        // Arrange
        var keyword = "art&design";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Creative art&design approach", ["creative"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOk_WithMultiWordKeyword()
    {
        // Arrange
        var keyword = "modern art";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Beautiful modern art design", ["art"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOk_WithStylesFromDifferentTypes()
    {
        // Arrange
        var keyword = "modern";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Abstract", "Modern abstract art", ["abstract"]),
            new("Style2", "Realistic", "Modern realistic design", ["realistic"]),
            new("Style3", "Minimalist", "Modern minimalist approach", ["minimal"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetByDescription_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var keyword = "test";
        var failureResult = CreateFailureResult<List<StyleResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during keyword search");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDescription_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var keyword = "test";
        var failureResult = CreateFailureResult<List<StyleResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDescription_ReturnsOk_WithLongKeyword()
    {
        // Arrange
        var longKeyword = new string('a', 100);
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", $"Description with {longKeyword}", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(longKeyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetByDescription_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var keyword = "modern";
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByDescriptionKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}