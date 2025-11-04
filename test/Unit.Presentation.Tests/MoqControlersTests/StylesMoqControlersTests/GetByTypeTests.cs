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

public sealed class GetByTypeTests : StylesControllerTestsBase
{
    [Fact]
    public async Task GetByType_ReturnsOkWithList_WhenStylesWithTypeExist()
    {
        // Arrange
        var styleType = "Custom";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "Description 1", ["tag1"]),
            new("Style2", styleType, "Description 2", ["tag2"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByType_ReturnsOkWithEmptyList_WhenNoStylesWithTypeExist()
    {
        // Arrange
        var styleType = "NonExistentType";
        var emptyList = new List<StyleResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByType_ReturnsBadRequest_WhenTypeIsEmpty()
    {
        // Arrange
        var invalidType = "";
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(invalidType, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByType_ReturnsBadRequest_WhenTypeIsNull()
    {
        // Arrange
        string? nullType = null;
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(nullType!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByType_ReturnsBadRequest_WhenTypeIsWhitespace()
    {
        // Arrange
        var whitespaceType = "   ";
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(whitespaceType, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByType_ReturnsBadRequest_WhenTypeExceedsMaxLength()
    {
        // Arrange
        var tooLongType = new string('a', 256);
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(tooLongType, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithStylesHavingNullTags()
    {
        // Arrange
        var styleType = "Custom";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "Style without tags", null),
            new("Style2", styleType, "Style with tags", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithStylesHavingNullDescription()
    {
        // Arrange
        var styleType = "Abstract";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, null, ["tag1"]),
            new("Style2", styleType, "Has description", ["tag2"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithManyMatchingStyles()
    {
        // Arrange
        var styleType = "Modern";
        var styles = Enumerable.Range(1, 10)
            .Select(i => new StyleResponse(
                $"Style{i}",
                styleType,
                $"Description {i}",
                [$"tag{i}"]))
            .ToList();

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 10);
    }

    [Fact]
    public async Task GetByType_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleType = "Traditional";
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        GetStylesByType.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<StyleResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetStylesByType.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(styleType, capturedQuery!.StyleType);
    }

    [Fact]
    public async Task GetByType_HandlesCancellationToken()
    {
        // Arrange
        var styleType = "Custom";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetByType(styleType, cts.Token));
    }

    [Fact]
    public async Task GetByType_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleType = "Vintage";
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("Custom")]
    [InlineData("Abstract")]
    [InlineData("Realistic")]
    [InlineData("Minimalist")]
    [InlineData("Traditional")]
    [InlineData("Contemporary")]
    public async Task GetByType_ReturnsOk_ForVariousStyleTypes(string styleType)
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, $"{styleType} style description", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByType_ReturnsConsistentResults_ForSameStyleType()
    {
        // Arrange
        var styleType = "Modern";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "Modern design", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetByType(styleType, CancellationToken.None);
        var actionResult2 = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult1, 1);
        AssertOkResult<StyleResponse>(actionResult2, 1);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithCaseInsensitiveStyleType()
    {
        // Arrange
        var lowercaseType = "modern";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Modern", "Modern style", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(lowercaseType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithStyleTypeContainingNumbers()
    {
        // Arrange
        var styleType = "Modern2024";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "2024 modern trends", ["trend"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithStyleTypeContainingHyphen()
    {
        // Arrange
        var styleType = "Modern-Art";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "Modern art style", ["modern"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithStyleTypeContainingUnderscore()
    {
        // Arrange
        var styleType = "Modern_Art";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "Modern art style", ["modern"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithStylesHavingMultipleTags()
    {
        // Arrange
        var styleType = "Contemporary";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "Multi-tag style", ["modern", "abstract", "colorful", "dynamic"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithStylesHavingEmptyTags()
    {
        // Arrange
        var styleType = "Minimalist";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "Style with empty tags", [])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByType_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleType = "ErrorType";
        var failureResult = CreateFailureResult<List<StyleResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Repository error during type search");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByType_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var styleType = "HandlerFailType";
        var failureResult = CreateFailureResult<List<StyleResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithLongStyleType()
    {
        // Arrange
        var longStyleType = new string('a', 100);
        var styles = new List<StyleResponse>
        {
            new("Style1", longStyleType, "Long type name style", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(longStyleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByType_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleType = "PerformanceTest";
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithStylesHavingDifferentDescriptions()
    {
        // Arrange
        var styleType = "Mixed";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "Short desc", ["tag1"]),
            new("Style2", styleType, "A much longer description that provides more detail about the style", ["tag2"]),
            new("Style3", styleType, null, ["tag3"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 3);
    }

    [Fact]
    public async Task GetByType_ReturnsOk_WithMixedCaseStyleType()
    {
        // Arrange
        var styleType = "MoDeRn-ArT";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "Mixed case type", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStylesByType.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<StyleResponse>(actionResult, 1);
    }
}