using Application.UseCases.Properties.Queries;
using Application.UseCases.Properties.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests;

public sealed class GetAllByVersionTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task GetAllByVersion_ReturnsOkWithList_WhenPropertiesExist()
    {
        // Arrange
        var version = "1.0";
        var properties = new List<PropertyQueryResponse>
        {
            new("1.0", "aspect", ["--ar", "--aspect"], "16:9", "1:1", "32:1", "Aspect ratio parameter"),
            new("1.0", "quality", ["--q", "--quality"], "1", "0.25", "2", "Quality parameter")
        };

        var result = Result.Ok(properties);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyQueryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsOkWithEmptyList_WhenNoPropertiesExist()
    {
        // Arrange
        var version = "1.0";
        var emptyList = new List<PropertyQueryResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyQueryResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsBadRequest_WhenVersionIsEmpty()
    {
        // Arrange
        var emptyVersion = string.Empty;
        var failureResult = CreateFailureResult<List<PropertyQueryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(emptyVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var nonExistentVersion = "99.0";
        var failureResult = CreateFailureResult<List<PropertyQueryResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Version '{nonExistentVersion}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(nonExistentVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var whitespaceVersion = "   ";
        var failureResult = CreateFailureResult<List<PropertyQueryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(whitespaceVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsBadRequest_WhenVersionIsNull()
    {
        // Arrange
        string? nullVersion = null;
        var failureResult = CreateFailureResult<List<PropertyQueryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(nullVersion!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsBadRequest_WhenVersionFormatIsInvalid()
    {
        // Arrange
        var invalidVersion = "invalid-version";
        var failureResult = CreateFailureResult<List<PropertyQueryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid version format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(invalidVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var version = "1.0";
        var failureResult = CreateFailureResult<List<PropertyQueryResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAllByVersion_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var version = "2.0";
        var properties = new List<PropertyQueryResponse>();
        var result = Result.Ok(properties);
        var senderMock = new Mock<ISender>();
        GetPropertiesByVersion.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<PropertyQueryResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetPropertiesByVersion.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(version, capturedQuery!.Version);
    }

    [Fact]
    public async Task GetAllByVersion_HandlesCancellationToken()
    {
        // Arrange
        var version = "1.0";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetAllPropertiesByVersion(version, cts.Token));
    }

    [Fact]
    public async Task GetAllByVersion_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var version = "1.0";
        var properties = new List<PropertyQueryResponse>();
        var result = Result.Ok(properties);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("1.0", 3)]
    [InlineData("2.0", 5)]
    [InlineData("5.2", 7)]
    [InlineData("6.0", 10)]
    public async Task GetAllByVersion_ReturnsOk_ForVariousVersionsAndCounts(string version, int count)
    {
        // Arrange
        var properties = Enumerable.Range(1, count)
            .Select(i => new PropertyQueryResponse(
                version,
                $"property{i}",
                [$"--p{i}"],
                "default",
                "min",
                "max",
                $"Description {i}"))
            .ToList();

        var result = Result.Ok(properties);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyQueryResponse>(actionResult, count);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsConsistentResults_ForSameVersion()
    {
        // Arrange
        var version = "1.0";
        var properties = new List<PropertyQueryResponse>
        {
            new("1.0", "aspect", ["--ar"], "16:9", "1:1", "32:1", "Aspect ratio")
        };
        var result = Result.Ok(properties);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);
        var actionResult2 = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<PropertyQueryResponse>(actionResult1, 1);
        AssertOkResult<PropertyQueryResponse>(actionResult2, 1);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsOk_WithPropertiesHavingMultipleParameters()
    {
        // Arrange
        var version = "1.0";
        var properties = new List<PropertyQueryResponse>
        {
            new("1.0", "aspect", ["--ar", "--aspect", "-a"], "16:9", "1:1", "32:1", "Aspect ratio with multiple params"),
            new("1.0", "quality", ["--q", "--quality"], "1", "0.25", "2", "Quality parameter")
        };

        var result = Result.Ok(properties);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyQueryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsOk_WithPropertiesHavingNullDescriptions()
    {
        // Arrange
        var version = "1.0";
        var properties = new List<PropertyQueryResponse>
        {
            new("1.0", "property1", ["--p1"], "default", "min", "max", null),
            new("1.0", "property2", ["--p2"], "default", "min", "max", "Has description")
        };

        var result = Result.Ok(properties);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyQueryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var version = "1.0";
        var failureResult = CreateFailureResult<List<PropertyQueryResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var version = "1.0";
        var failureResult = CreateFailureResult<List<PropertyQueryResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsOk_WithLargeNumberOfProperties()
    {
        // Arrange
        var version = "1.0";
        var largeList = Enumerable.Range(1, 50)
            .Select(i => new PropertyQueryResponse(
                version,
                $"property{i}",
                [$"--p{i}"],
                $"default{i}",
                $"min{i}",
                $"max{i}",
                $"Description {i}"))
            .ToList();

        var result = Result.Ok(largeList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyQueryResponse>(actionResult, 50);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsOk_ForDifferentVersionFormats()
    {
        // Arrange - Testing different valid version formats
        var versions = new[] { "1", "2.0", "5.2", "6.0", "niji-5" };

        foreach (var version in versions)
        {
            var properties = new List<PropertyQueryResponse>
            {
                new(version, "property", ["--p"], "default", "min", "max", "Description")
            };

            var result = Result.Ok(properties);
            var senderMock = new Mock<ISender>();
            senderMock
                .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var controller = CreateController(senderMock);

            // Act
            var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

            // Assert
            actionResult.Should().NotBeNull();
            AssertOkResult<PropertyQueryResponse>(actionResult, 1);
        }
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsOk_WithPropertiesHavingSpecialCharactersInNames()
    {
        // Arrange
        var version = "1.0";
        var properties = new List<PropertyQueryResponse>
        {
            new("1.0", "aspect-ratio", ["--ar"], "16:9", "1:1", "32:1", "Aspect with hyphen"),
            new("1.0", "quality_level", ["--q"], "1", "0.25", "2", "Quality with underscore")
        };

        var result = Result.Ok(properties);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PropertyQueryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAllByVersion_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var version = "1.0";
        var properties = new List<PropertyQueryResponse>();
        var result = Result.Ok(properties);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetPropertiesByVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}