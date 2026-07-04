using Application.Abstractions.IRepository;
using Application.Extensions;
using FluentAssertions;
using Moq;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Unit.Application.Tests.Extensions;

public class WorkflowPipelineHistoryValidationTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    #region IfHistoryRecordsLimitNotGreaterThanZero Tests

    [Fact]
    public async Task IfHistoryRecordsLimitNotGreaterThanZero_ShouldAddError_WhenCountIsZero()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));

        // Act
        var result = await pipelineTask.IfHistoryRecordsLimitNotGreaterThanZero(0);

        // Assert
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("History count must be greater than zero");
    }

    [Fact]
    public async Task IfHistoryRecordsLimitNotGreaterThanZero_ShouldAddError_WhenCountIsNegative()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));

        // Act
        var result = await pipelineTask.IfHistoryRecordsLimitNotGreaterThanZero(-5);

        // Assert
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("History count must be greater than zero");
    }

    [Fact]
    public async Task IfHistoryRecordsLimitNotGreaterThanZero_ShouldNotAddError_WhenCountIsOne()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));

        // Act
        var result = await pipelineTask.IfHistoryRecordsLimitNotGreaterThanZero(1);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task IfHistoryRecordsLimitNotGreaterThanZero_ShouldNotAddError_WhenCountIsPositive()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));

        // Act
        var result = await pipelineTask.IfHistoryRecordsLimitNotGreaterThanZero(5);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task IfHistoryRecordsLimitNotGreaterThanZero_ShouldNotAddError_WhenCountIsLargeNumber()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));

        // Act
        var result = await pipelineTask.IfHistoryRecordsLimitNotGreaterThanZero(int.MaxValue);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task IfHistoryRecordsLimitNotGreaterThanZero_ShouldBreakPipeline_WhenBreakOnErrorIsTrue()
    {
        // Arrange
        var existingError = new Error("Existing error");
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([existingError], true));

        // Act
        var result = await pipelineTask.IfHistoryRecordsLimitNotGreaterThanZero(0);

        // Assert
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be("Existing error");
    }

    [Fact]
    public async Task IfHistoryRecordsLimitNotGreaterThanZero_ShouldPreserveExistingErrors_WhenAddingNewError()
    {
        // Arrange
        var existingError = new Error("Existing error");
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([existingError], false));

        // Act
        var result = await pipelineTask.IfHistoryRecordsLimitNotGreaterThanZero(0);

        // Assert
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().ContainSingle(e => e.Message.Contains("History count must be greater than zero"));
        result.Errors.Should().ContainSingle(e => e.Message == "Existing error");
    }

    #endregion

    #region IfHistoryCountExceedsAvailable Tests

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldAddError_WhenRequestedExceedsAvailable()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken))
            .ReturnsAsync(Result.Ok(3));

        // Act
        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Requested 5 records, but only 3 are available");
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldNotAddError_WhenRequestedEqualsAvailable()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken))
            .ReturnsAsync(Result.Ok(5));

        // Act
        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldNotAddError_WhenRequestedIsWithinLimit()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken))
            .ReturnsAsync(Result.Ok(10));

        // Act
        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldAddError_WhenRequestedExceedsByOne()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken))
            .ReturnsAsync(Result.Ok(4));

        // Act
        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Requested 5 records, but only 4 are available");
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldHandleZeroAvailableRecords()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken))
            .ReturnsAsync(Result.Ok(0));

        // Act
        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Requested 5 records, but only 0 are available");
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldAddRepositoryError_WhenRepositoryFails()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repoError = new Error("Repository connection failed");
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken))
            .ReturnsAsync(Result.Fail<int>(repoError));

        // Act
        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be("Repository connection failed");
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldAddMultipleRepositoryErrors()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repoError1 = new Error("Database error 1");
        var repoError2 = new Error("Database error 2");
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken))
            .ReturnsAsync(Result.Fail<int>([repoError1, repoError2]));

        // Act
        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(e => e.Message == "Database error 1");
        result.Errors.Should().Contain(e => e.Message == "Database error 2");
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldBreakPipeline_WhenBreakOnErrorIsTrue()
    {
        // Arrange
        var existingError = new Error("Existing validation error");
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([existingError], true));
        var repo = new Mock<IPromptHistoryRepository>();

        // Act
        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be("Existing validation error");
        repo.Verify(r => r.CalculateHistoricalRecordCountAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldPreserveExistingErrors_WhenAddingNewError()
    {
        // Arrange
        var existingError = new Error("Existing error");
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([existingError], false));
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken))
            .ReturnsAsync(Result.Ok(2));

        // Act
        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().ContainSingle(e => e.Message == "Existing error");
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Requested 5 records, but only 2 are available"));
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldCallRepository_WithProvidedCancellationToken()
    {
        // Arrange
        var customCancellationToken = new CancellationTokenSource().Token;
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(customCancellationToken))
            .ReturnsAsync(Result.Ok(10));

        // Act
        await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, customCancellationToken);

        // Assert
        repo.Verify(
            r => r.CalculateHistoricalRecordCountAsync(customCancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldHandleLargeRequestedCount()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken))
            .ReturnsAsync(Result.Ok(100));

        // Act
        var result = await pipelineTask.IfHistoryCountExceedsAvailable(int.MaxValue, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain($"HistoryRequestedCount requested {int.MaxValue}, but only 100 available");
    }

    #endregion

    #region Pipeline Chain Tests

    [Fact]
    public async Task ValidationChain_ShouldStopOnFirstError_WhenBreakOnErrorIsTrue()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repo = new Mock<IPromptHistoryRepository>();

        // Act
        var result = await pipelineTask
            .IfHistoryRecordsLimitNotGreaterThanZero(0)
            .IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().HaveCount(1)
            .And.AllSatisfy(e => e.Message.Should().Contain("History count must be greater than zero"));
        repo.Verify(r => r.CalculateHistoricalRecordCountAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidationChain_ShouldAccumulateMultipleErrors_WhenBreakOnErrorIsFalse()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken))
            .ReturnsAsync(Result.Ok(2));

        // Act
        var result = await pipelineTask
            .IfHistoryRecordsLimitNotGreaterThanZero(0)
            .IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        // Assert
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().ContainSingle(e => e.Message.Contains("History count must be greater than zero"));
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Requested 5 records, but only 2 are available"));
    }

    #endregion
}