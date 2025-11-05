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
        var styleName = "ModernArt";
        var version = "1.0";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com/image1.jpg", styleName, version),
            new("http://example2.com/image2.png", styleName, version)
        };

        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsOkWithEmptyList_WhenNoMatchingLinks()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "99.0";
        var emptyList = new List<ExampleLinkResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenStyleNameIsInvalid()
    {
        // Arrange
        var invalidStyleName = string.Empty;
        var version = "1.0";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(invalidStyleName, version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenVersionIsInvalid()
    {
        // Arrange
        var styleName = "ModernArt";
        var invalidVersion = string.Empty;
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, invalidVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenBothParametersAreInvalid()
    {
        // Arrange
        var invalidStyleName = string.Empty;
        var invalidVersion = string.Empty;
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name and version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(invalidStyleName, invalidVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var nonExistentStyleName = "NonExistentStyle";
        var version = "1.0";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Style '{nonExistentStyleName}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(nonExistentStyleName, version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var styleName = "ModernArt";
        var nonExistentVersion = "99.0";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Version '{nonExistentVersion}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, nonExistentVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsNotFound_WhenBothStyleAndVersionDoNotExist()
    {
        // Arrange
        var nonExistentStyleName = "NonExistentStyle";
        var nonExistentVersion = "99.0";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style and version not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(nonExistentStyleName, nonExistentVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var whitespaceStyleName = "   ";
        var version = "1.0";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(whitespaceStyleName, version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var styleName = "ModernArt";
        var whitespaceVersion = "   ";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, whitespaceVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var tooLongStyleName = new string('A', 256);
        var version = "1.0";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(tooLongStyleName, version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "1.0";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var version = "2.0";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", styleName, version)
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        GetExampleLinksByStyleAndVersion.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<ExampleLinkResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetExampleLinksByStyleAndVersion.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(styleName, capturedQuery!.StyleName);
        Assert.Equal(version, capturedQuery.Version);
    }

    [Fact]
    public async Task GetByStyleAndVersion_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "1.0";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetByStyleAndVersion(styleName, version, cts.Token));
    }

    [Fact]
    public async Task GetByStyleAndVersion_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "1.0";
        var list = new List<ExampleLinkResponse>();
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

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

        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, count);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "1.0";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", styleName, version)
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);
        var actionResult2 = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult1, 1);
        AssertOkResult<ExampleLinkResponse>(actionResult2, 1);
    }

    [Fact]
    public async Task GetByStyleAndVersion_HandlesSpecialCharactersInStyleName()
    {
        // Arrange
        var styleNameWithSpecialChars = "Modern-Art_2024";
        var version = "1.0";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", styleNameWithSpecialChars, version)
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleNameWithSpecialChars, version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 1);
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
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", styleName, version)
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "1.0";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "1.0";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_HandlesCaseInsensitiveStyleNames()
    {
        // Arrange
        var lowercaseStyleName = "modernart";
        var version = "1.0";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example.com/image.jpg", lowercaseStyleName, version)
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(lowercaseStyleName, version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsOk_WithMultipleLinksForSameStyleAndVersion()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "1.0";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com/image1.jpg", styleName, version),
            new("http://example2.com/image2.png", styleName, version),
            new("http://example3.com/image3.jpeg", styleName, version),
            new("http://example4.com/image4.webp", styleName, version)
        };
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<ExampleLinkResponse>(actionResult, 4);
    }

    [Fact]
    public async Task GetByStyleAndVersion_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "1.0";
        var list = new List<ExampleLinkResponse>();
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetExampleLinksByStyleAndVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}