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

public sealed class GetAllStylesTests : StylesControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsOkWithList_WhenStylesExist()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("ModernArt", "Custom", "Modern art style", ["abstract", "contemporary"]),
            new("ClassicArt", "Traditional", "Classic art style", ["vintage", "traditional"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithEmptyList_WhenNoStylesExist()
    {
        // Arrange
        var emptyList = new List<StyleResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

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
        var failureResult = CreateFailureResult<List<StyleResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenApplicationLayerErrorOccurs()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<StyleResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Application layer error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

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
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        GetAllStyles.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<StyleResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetAllStyles.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery.Should().BeSameAs(GetAllStyles.Query.Singletone);
    }

    [Fact]
    public async Task GetAll_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions.Awaiting(() => controller.GetAll(cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetAll_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithStylesHavingNullDescriptions()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("MinimalStyle", "Modern", null, ["minimal"]),
            new("CleanStyle", "Contemporary", "Has description", ["clean"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithStylesHavingNullTags()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("MinimalStyle", "Modern", "Simple minimal style", null),
            new("CleanStyle", "Contemporary", null, null)
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithStylesHavingEmptyTagsList()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("Style1", "Modern", "Description", []),
            new("Style2", "Contemporary", "Description", ["tag1"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithLargeNumberOfStyles()
    {
        // Arrange
        var styles = new List<StyleResponse>();
        for (int i = 1; i <= 100; i++)
        {
            styles.Add(new StyleResponse(
                $"Style{i}",
                i % 2 == 0 ? "Modern" : "Traditional",
                $"Description for style {i}",
                [$"tag{i}", $"category{i % 5}"]
            ));
        }

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(100);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithStylesHavingVariousTypes()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("ArtStyle1", "Abstract", "Abstract art style", ["abstract", "modern"]),
            new("ArtStyle2", "Realistic", "Realistic art style", ["realistic", "detailed"]),
            new("ArtStyle3", "Minimalist", "Minimal design style", ["minimal", "clean"]),
            new("ArtStyle4", "Vintage", "Retro vintage style", ["vintage", "retro"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithStylesHavingMultipleTags()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("Style1", "Modern", "Description", ["tag1", "tag2", "tag3", "tag4", "tag5"]),
            new("Style2", "Traditional", "Description", ["vintage"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsConsistentResults_WhenCalledMultipleTimes()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("Style1", "Modern", "Description", ["tag1"])
        };
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetAll(CancellationToken.None);
        var actionResult2 = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult1.Should().BeOkResult().WithCount(1);
        actionResult2.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithStylesHavingLongDescriptions()
    {
        // Arrange
        var longDescription = new string('a', 500);
        var styles = new List<StyleResponse>
        {
            new("Style1", "Modern", longDescription, ["tag1"]),
            new("Style2", "Traditional", "Short", ["tag2"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithStylesHavingSpecialCharacters()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("Modern-Art_Style", "Custom", "Description with special chars: @#$%", ["tag-1", "tag_2"]),
            new("Classic Art", "Traditional", "Simple description", ["vintage"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithStylesHavingDifferentCases()
    {
        // Arrange
        var styles = new List<StyleResponse>
        {
            new("UPPERCASE", "Modern", "Description", ["TAG1"]),
            new("lowercase", "Traditional", "Description", ["tag2"]),
            new("MixedCase", "Contemporary", "Description", ["Tag3"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<StyleResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during retrieval");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<StyleResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task GetAll_ReturnsOk_ForVariousStyleCounts(int count)
    {
        // Arrange
        var styles = Enumerable.Range(1, count)
            .Select(i => new StyleResponse(
                $"Style{i}",
                "Modern",
                $"Description {i}",
                [$"tag{i}"]
            ))
            .ToList();

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(count);
    }

    [Fact]
    public async Task GetAll_UsesSingletonPattern_VerifiesSameInstance()
    {
        // Arrange
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        var capturedQueries = new List<GetAllStyles.Query>();

        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<StyleResponse>>>, CancellationToken>((query, ct) =>
            {
                if (query is GetAllStyles.Query q)
                    capturedQueries.Add(q);
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);
        await controller.GetAll(CancellationToken.None);
        await controller.GetAll(CancellationToken.None);

        // Assert
        capturedQueries.Should().HaveCount(3);
        capturedQueries.Should().AllSatisfy(q => q.Should().BeSameAs(GetAllStyles.Query.Singletone));
    }

    [Fact]
    public async Task GetAll_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styles = new List<StyleResponse>();
        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllStyles.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}