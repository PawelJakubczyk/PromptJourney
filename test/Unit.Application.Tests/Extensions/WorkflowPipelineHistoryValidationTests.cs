using Application.Abstractions.IRepository;
using Application.Extensions;
using FluentAssertions;
using FluentResults;
using Moq;
using Utilities.Workflows;

namespace Unit.Application.Tests.Extensions;

public class WorkflowPipelineHistoryValidationTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact]
    public async Task IfHistoryLimitNotGreaterThanZero_ShouldAddError_WhenCountIsZero()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));

        var result = await pipelineTask.IfHistoryLimitNotGreaterThanZero(0);

        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("History count must be greater than zero");
    }

    [Fact]
    public async Task IfHistoryLimitNotGreaterThanZero_ShouldNotAddError_WhenCountIsPositive()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));

        var result = await pipelineTask.IfHistoryLimitNotGreaterThanZero(5);

        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldAddError_WhenRequestedExceedsAvailable()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));

        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken)).ReturnsAsync(Result.Ok(3));

        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Requested 5 records, but only 3 are available");
    }

    [Fact]
    public async Task IfHistoryCountExceedsAvailable_ShouldNotAddError_WhenRequestedIsWithinLimit()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));

        var repo = new Mock<IPromptHistoryRepository>();
        repo.Setup(r => r.CalculateHistoricalRecordCountAsync(_cancellationToken)).ReturnsAsync(Result.Ok(10));

        var result = await pipelineTask.IfHistoryCountExceedsAvailable(5, repo.Object, _cancellationToken);

        result.Errors.Should().BeEmpty();
    }
}
