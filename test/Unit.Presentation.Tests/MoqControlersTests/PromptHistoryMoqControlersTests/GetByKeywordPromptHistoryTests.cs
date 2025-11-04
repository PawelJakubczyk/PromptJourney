using Application.UseCases.PromptHistory.Queries;
using Application.UseCases.PromptHistory.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests;

public sealed class GetByKeywordPromptHistoryTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task GetByKeyword_ReturnsOkWithList_WhenRecordsWithKeywordExist()
    {
        // Arrange
        var keyword = "landscape";
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "A beautiful landscape with mountains", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Urban landscape at sunset", "2.0", DateTime.UtcNow.AddHours(-1))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsOkWithEmptyList_WhenNoRecordsMatchKeyword()
    {
        // Arrange
        var keyword = "nonexistent";
        var emptyList = new List<PromptHistoryResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsBadRequest_WhenKeywordIsEmpty()
    {
        // Arrange
        var emptyKeyword = string.Empty;
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Keyword cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(emptyKeyword, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsBadRequest_WhenKeywordIsWhitespace()
    {
        // Arrange
        var whitespaceKeyword = "   ";
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Keyword cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(whitespaceKeyword, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsBadRequest_WhenKeywordIsNull()
    {
        // Arrange
        string? nullKeyword = null;
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Keyword cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(nullKeyword!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsBadRequest_WhenKeywordExceedsMaxLength()
    {
        // Arrange
        var tooLongKeyword = new string('a', 256);
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Keyword exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(tooLongKeyword, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var keyword = "landscape";
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByKeyword_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var keyword = "test";
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        GetHistoryRecordsByPromptKeyword.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<PromptHistoryResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetHistoryRecordsByPromptKeyword.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(keyword, capturedQuery!.Keyword);
    }

    [Fact]
    public async Task GetByKeyword_HandlesCancellationToken()
    {
        // Arrange
        var keyword = "landscape";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetByKeyword(keyword, cts.Token));
    }

    [Fact]
    public async Task GetByKeyword_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var keyword = "test";
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("landscape")]
    [InlineData("portrait")]
    [InlineData("art")]
    [InlineData("modern")]
    [InlineData("city")]
    public async Task GetByKeyword_ReturnsOk_ForVariousValidKeywords(string keyword)
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), $"Prompt containing {keyword}", "1.0", DateTime.UtcNow)
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsConsistentResults_ForSameKeyword()
    {
        // Arrange
        var keyword = "landscape";
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Beautiful landscape", "1.0", DateTime.UtcNow)
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetByKeyword(keyword, CancellationToken.None);
        var actionResult2 = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult1, 1);
        AssertOkResult<PromptHistoryResponse>(actionResult2, 1);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsOk_WithCaseInsensitiveKeyword()
    {
        // Arrange
        var lowercaseKeyword = "landscape";
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Beautiful LANDSCAPE", "1.0", DateTime.UtcNow)
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(lowercaseKeyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsOk_WithPartialKeywordMatch()
    {
        // Arrange
        var keyword = "land";
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Beautiful landscape", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Inland waterways", "1.0", DateTime.UtcNow.AddMinutes(-5))
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsOk_WithSpecialCharactersInKeyword()
    {
        // Arrange
        var keyword = "art-style";
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Modern art-style design", "1.0", DateTime.UtcNow)
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsOk_WithNumbersInKeyword()
    {
        // Arrange
        var keyword = "2024";
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Art trends for 2024", "1.0", DateTime.UtcNow)
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsOk_WithMultipleRecordsMatchingKeyword()
    {
        // Arrange
        var keyword = "modern";
        var historyRecords = Enumerable.Range(1, 10)
            .Select(i => new PromptHistoryResponse(
                Guid.NewGuid(),
                $"Modern design {i}",
                "1.0",
                DateTime.UtcNow.AddMinutes(-i)))
            .ToList();

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 10);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsOk_WithRecordsFromDifferentVersions()
    {
        // Arrange
        var keyword = "landscape";
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Landscape v1", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Landscape v2", "2.0", DateTime.UtcNow.AddMinutes(-1)),
            new(Guid.NewGuid(), "Landscape v5", "5.2", DateTime.UtcNow.AddMinutes(-2))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 3);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var keyword = "landscape";
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var keyword = "landscape";
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsOk_WithUnicodeCharactersInKeyword()
    {
        // Arrange
        var unicodeKeyword = "??";
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Beautiful ?? landscape", "1.0", DateTime.UtcNow)
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(unicodeKeyword, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByKeyword_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var keyword = "landscape";
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryRecordsByPromptKeyword.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}