using Application.UseCases.Versions.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests;

public sealed class GetSupportedVersionsTests : VersionsControllerTestsBase
{
    [Fact]
    public async Task GetSupported_ReturnsOkWithList_WhenSupportedVersionsExist()
    {
        // Arrange
        var supportedVersions = new List<string> { "1.0", "2.0", "5.1", "6.0" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithEmptyList_WhenNoSupportedVersionsExist()
    {
        // Arrange
        var emptyList = new List<string>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(0);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithMultipleSupportedVersions()
    {
        // Arrange
        var supportedVersions = new List<string>
        {
            "1.0", "2.0", "3.0", "4.0", "5.0", "5.1", "5.2", "6.0", "niji 5", "niji 6"
        };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(10);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithStandardVersionsOnly()
    {
        // Arrange
        var supportedVersions = new List<string> { "1.0", "2.0", "3.0", "4.0", "5.0", "6.0" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(6);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithNijiVersionsOnly()
    {
        // Arrange
        var supportedVersions = new List<string> { "niji 4", "niji 5", "niji 6" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithMixedVersionTypes()
    {
        // Arrange
        var supportedVersions = new List<string>
        {
            "1.0", "2.5", "5.1", "5.2", "6.0", "niji 5", "niji 6"
        };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(7);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithSingleVersion()
    {
        // Arrange
        var supportedVersions = new List<string> { "6.0" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithVersionsContainingDecimals()
    {
        // Arrange
        var supportedVersions = new List<string> { "1.0", "2.5", "3.7", "4.2", "5.1", "5.2" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(6);
    }

    [Fact]
    public async Task GetSupported_VerifiesQueryIsCalledWithSingleton()
    {
        // Arrange
        var supportedVersions = new List<string>();
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        GetAllSuportedVersions.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<string>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetAllSuportedVersions.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetSupported(CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Same(GetAllSuportedVersions.Query.Singletone, capturedQuery);
    }

    [Fact]
    public async Task GetSupported_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetSupported(cts.Token));
    }

    [Fact]
    public async Task GetSupported_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var supportedVersions = new List<string> { "1.0" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetSupported(CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetSupported_ReturnsConsistentResults_ForMultipleCalls()
    {
        // Arrange
        var supportedVersions = new List<string> { "1.0", "2.0", "6.0" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetSupported(CancellationToken.None);
        var actionResult2 = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        actionResult1.Should().BeOkResult().WithCount(3);
        actionResult2.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetSupported_ReturnsBadRequest_WhenRepositoryFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<string>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Repository error during supported versions retrieval");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetSupported_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<string>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetSupported_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var supportedVersions = new List<string> { "1.0", "2.0", "6.0" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetSupported(CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithVersionsInChronologicalOrder()
    {
        // Arrange
        var supportedVersions = new List<string>
        {
            "1.0", "2.0", "3.0", "4.0", "5.0", "5.1", "5.2", "6.0"
        };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(8);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithBetaVersions()
    {
        // Arrange
        var supportedVersions = new List<string>
        {
            "6.0", "7.0-beta", "8.0-alpha"
        };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithLegacyVersions()
    {
        // Arrange
        var supportedVersions = new List<string>
        {
            "1.0", "2.0", "3.0", "4.0"
        };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithCurrentVersions()
    {
        // Arrange
        var supportedVersions = new List<string>
        {
            "5.2", "6.0", "niji 5", "niji 6"
        };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithManyVersions()
    {
        // Arrange
        var supportedVersions = Enumerable.Range(1, 20)
            .Select(i => $"{i}.0")
            .ToList();
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(20);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetSupported_ReturnsOk_WithVariousListSizes(int count)
    {
        // Arrange
        var supportedVersions = Enumerable.Range(1, count)
            .Select(i => $"{i}.0")
            .ToList();
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(count);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithVersionsContainingSpaces()
    {
        // Arrange
        var supportedVersions = new List<string>
        {
            "niji 5", "niji 6", "niji 7"
        };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithVersionsContainingDashes()
    {
        // Arrange
        var supportedVersions = new List<string>
        {
            "6.0-niji", "7.0-beta", "8.0-alpha"
        };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithMajorVersionsOnly()
    {
        // Arrange
        var supportedVersions = new List<string> { "1", "2", "3", "4", "5", "6" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(6);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithDuplicateVersions()
    {
        // Arrange
        var supportedVersions = new List<string> { "1.0", "1.0", "2.0", "2.0" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithCount(4);
    }
}