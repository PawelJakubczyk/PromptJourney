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

public sealed class GetByTagsTests : StylesControllerTestsBase
{
    [Fact]
    public async Task GetByTags_ReturnsOkWithList_WhenStylesWithTagsExist()
    {
        // Arrange
        var tags = new List<string> { "abstract", "modern" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Description 1", ["abstract", "contemporary"]),
            new("Style2", "Custom", "Description 2", ["modern", "minimalist"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByTags_ReturnsOkWithEmptyList_WhenNoStylesMatchTags()
    {
        // Arrange
        var tags = new List<string> { "nonexistent" };
        var emptyList = new List<StyleResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByTags_ReturnsBadRequest_WhenTagsListIsEmpty()
    {
        // Arrange
        var emptyTags = new List<string>();
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tags list cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(emptyTags, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByTags_ReturnsBadRequest_WhenTagsListIsNull()
    {
        // Arrange
        List<string>? nullTags = null;
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tags list cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(nullTags!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByTags_ReturnsBadRequest_WhenTagContainsEmptyString()
    {
        // Arrange
        var invalidTags = new List<string> { "valid", "" };
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(invalidTags, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByTags_ReturnsBadRequest_WhenTagContainsWhitespace()
    {
        // Arrange
        var invalidTags = new List<string> { "valid", "   " };
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(invalidTags, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByTags_ReturnsBadRequest_WhenTagExceedsMaxLength()
    {
        // Arrange
        var invalidTags = new List<string> { new string('a', 256) };
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(invalidTags, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithSingleTag()
    {
        // Arrange
        var tags = new List<string> { "abstract" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Abstract style", ["abstract", "modern"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithMultipleTags()
    {
        // Arrange
        var tags = new List<string> { "abstract", "modern", "colorful" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Multi-tag style", ["abstract", "modern", "colorful"]),
            new("Style2", "Custom", "Another style", ["modern", "colorful"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithManyMatchingStyles()
    {
        // Arrange
        var tags = new List<string> { "modern" };
        var styles = Enumerable.Range(1, 10)
            .Select(i => new StyleResponse(
                $"Style{i}",
                "Custom",
                $"Modern style {i}",
                ["modern", $"tag{i}"]))
            .ToList();

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 10);
    }

    [Fact]
    public async Task GetByTags_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var tags = new List<string> { "abstract", "modern" };
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        GetStylesByTags.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<StyleResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetStylesByTags.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(tags, capturedQuery!.Tags);
    }

    [Fact]
    public async Task GetByTags_HandlesCancellationToken()
    {
        // Arrange
        var tags = new List<string> { "test" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetByTags(tags, cts.Token));
    }

    [Fact]
    public async Task GetByTags_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var tags = new List<string> { "test" };
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("modern")]
    [InlineData("abstract")]
    [InlineData("vintage")]
    [InlineData("minimalist")]
    [InlineData("artistic")]
    public async Task GetByTags_ReturnsOk_ForVariousSingleTags(string tag)
    {
        // Arrange
        var tags = new List<string> { tag };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", $"Style with {tag} tag", [tag, "other"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByTags_ReturnsConsistentResults_ForSameTags()
    {
        // Arrange
        var tags = new List<string> { "modern", "abstract" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Test style", ["modern", "abstract"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetByTags(tags, CancellationToken.None);
        var actionResult2 = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult1, 1);
        AssertOkResult<StyleResponse>(actionResult2, 1);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithCaseInsensitiveTags()
    {
        // Arrange
        var tags = new List<string> { "MODERN", "abstract" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Case test style", ["modern", "abstract"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithStylesHavingNullTags()
    {
        // Arrange
        var tags = new List<string> { "modern" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Style with null tags", null),
            new("Style2", "Custom", "Style with tags", ["modern"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithTagsContainingNumbers()
    {
        // Arrange
        var tags = new List<string> { "2024", "modern" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "2024 trends", ["2024", "modern"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithTagsContainingHyphens()
    {
        // Arrange
        var tags = new List<string> { "modern-art", "abstract-design" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Hyphenated tags style", ["modern-art", "abstract-design"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithTagsContainingUnderscores()
    {
        // Arrange
        var tags = new List<string> { "modern_art", "abstract_design" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Underscore tags style", ["modern_art", "abstract_design"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithStylesFromDifferentTypes()
    {
        // Arrange
        var tags = new List<string> { "modern" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Abstract", "Modern abstract", ["modern"]),
            new("Style2", "Realistic", "Modern realistic", ["modern"]),
            new("Style3", "Minimalist", "Modern minimal", ["modern"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 3);
    }

    [Fact]
    public async Task GetByTags_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var tags = new List<string> { "test" };
        var failureResult = CreateFailureResult<List<StyleResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Repository error during tag search");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByTags_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var tags = new List<string> { "test" };
        var failureResult = CreateFailureResult<List<StyleResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithDuplicateTags()
    {
        // Arrange
        var tags = new List<string> { "modern", "modern", "abstract" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Test style", ["modern", "abstract"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithLongTagsList()
    {
        // Arrange
        var tags = Enumerable.Range(1, 20).Select(i => $"tag{i}").ToList();
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Many tags style", tags)
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByTags_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var tags = new List<string> { "modern" };
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetByTags_ReturnsOk_WithMixedCaseAndSpecialCharacterTags()
    {
        // Arrange
        var tags = new List<string> { "Modern-Art_2024", "ABSTRACT", "vintage" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Mixed tags style", ["Modern-Art_2024", "ABSTRACT", "vintage"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByTags.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }
}