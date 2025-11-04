using Application.UseCases.Versions.Queries;
using Application.UseCases.Versions.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests;

public sealed class GetAllVersionsTests : VersionsControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsOkWithList_WhenVersionsExist()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow, "Version 1.0"),
            new("2.0", "--v 2.0", DateTime.UtcNow, "Version 2.0")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithEmptyList_WhenNoVersionsExist()
    {
        // Arrange
        var emptyList = new List<VersionResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenRepositoryFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<VersionResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithMultipleVersions()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow.AddYears(-3), "Version 1.0"),
            new("2.0", "--v 2.0", DateTime.UtcNow.AddYears(-2), "Version 2.0"),
            new("3.0", "--v 3.0", DateTime.UtcNow.AddYears(-1), "Version 3.0"),
            new("4.0", "--v 4.0", DateTime.UtcNow.AddMonths(-6), "Version 4.0"),
            new("5.0", "--v 5.0", DateTime.UtcNow.AddMonths(-3), "Version 5.0")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 5);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithManyVersions()
    {
        // Arrange
        var versions = Enumerable.Range(1, 10)
            .Select(i => new VersionResponse(
                $"{i}.0",
                $"--v {i}.0",
                DateTime.UtcNow.AddMonths(-i),
                $"Version {i}.0"))
            .ToList();

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 10);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithVersionsHavingNullDescriptions()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow, null),
            new("2.0", "--v 2.0", DateTime.UtcNow, "Version 2.0"),
            new("3.0", "--v 3.0", DateTime.UtcNow, null)
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 3);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithVersionsHavingNullReleaseDates()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", null, "Version 1.0"),
            new("2.0", "--v 2.0", DateTime.UtcNow, "Version 2.0"),
            new("3.0", "--v 3.0", null, "Version 3.0")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 3);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithMixedVersionTypes()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow, "Standard version 1.0"),
            new("2.5", "--v 2.5", DateTime.UtcNow, "Standard version 2.5"),
            new("niji 5", "--niji 5", DateTime.UtcNow, "Niji version 5"),
            new("niji 6", "--niji 6", DateTime.UtcNow, "Niji version 6"),
            new("6.0", "--v 6.0", DateTime.UtcNow, "Standard version 6.0")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 5);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithVersionsHavingSpecialCharactersInDescription()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow, "Version with spéciál characters 🎨"),
            new("2.0", "--v 2.0", DateTime.UtcNow, "Version with symbols @#$%^&*()")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_VerifiesQueryIsCalledWithSingleton()
    {
        // Arrange
        var versions = new List<VersionResponse>();
        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        GetAllVersions.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<VersionResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetAllVersions.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Same(GetAllVersions.Query.Singletone, capturedQuery);
    }

    [Fact]
    public async Task GetAll_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetAll(cts.Token));
    }

    [Fact]
    public async Task GetAll_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var versions = new List<VersionResponse>();
        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsConsistentResults_ForMultipleCalls()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow, "Version 1.0")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetAll(CancellationToken.None);
        var actionResult2 = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult1, 1);
        AssertOkResult<VersionResponse>(actionResult2, 1);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<VersionResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithVersionsInChronologicalOrder()
    {
        // Arrange
        var date1 = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var date2 = new DateTime(2021, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var date3 = new DateTime(2022, 12, 1, 0, 0, 0, DateTimeKind.Utc);

        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", date1, "First version"),
            new("2.0", "--v 2.0", date2, "Second version"),
            new("3.0", "--v 3.0", date3, "Third version")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 3);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithVersionsHavingLongDescriptions()
    {
        // Arrange
        var longDescription = new string('A', 1000) + " This is a very long description for testing purposes.";
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow, longDescription),
            new("2.0", "--v 2.0", DateTime.UtcNow, "Short description")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithBetaAndAlphaVersions()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("7.0-alpha", "--v 7.0", DateTime.UtcNow, "Alpha version"),
            new("7.0-beta", "--v 7.0", DateTime.UtcNow, "Beta version"),
            new("7.0", "--v 7.0", DateTime.UtcNow, "Stable version")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 3);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithVersionsHavingDifferentParameters()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow, "Version 1"),
            new("2.0", "--version 2.0", DateTime.UtcNow, "Version 2"),
            new("niji 5", "--niji 5", DateTime.UtcNow, "Niji 5"),
            new("6.0", "--v 6", DateTime.UtcNow, "Version 6")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 4);
    }

    [Fact]
    public async Task GetAll_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var versions = new List<VersionResponse>();
        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithVersionsHavingMinimalData()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", null, null),
            new("2.0", "--v 2.0", null, null)
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithVersionsHavingCompleteData()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow, "Complete version with all fields populated"),
            new("2.0", "--v 2.0", DateTime.UtcNow, "Another complete version")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithSingleVersion()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow, "Only version")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithVersionsHavingFutureReleaseDates()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddMonths(6);
        var versions = new List<VersionResponse>
        {
            new("8.0", "--v 8.0", futureDate, "Future version"),
            new("9.0", "--v 9.0", futureDate.AddMonths(3), "Another future version")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllVersions.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, 2);
    }
}