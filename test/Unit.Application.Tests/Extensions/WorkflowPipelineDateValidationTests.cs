using Application.Extensions;
using Utilities.Validation;
using FluentAssertions;

namespace Unit.Application.Tests.Extensions;

public class WorkflowPipelineDateValidationTests
{
    [Fact]
    public async Task IfDateInFuture_ShouldAddError_WhenDateIsInFuture()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var futureDate = DateTime.UtcNow.AddDays(1);

        var result = await pipelineTask.IfDateInFuture(futureDate);

        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("cannot be in the future");
    }

    [Fact]
    public async Task IfDateInFuture_ShouldNotAddError_WhenDateIsInPast()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var pastDate = DateTime.UtcNow.AddDays(-1);

        var result = await pipelineTask.IfDateInFuture(pastDate);

        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task IfDateRangeNotChronological_ShouldAddError_WhenFromIsAfterTo()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var from = new DateTime(2025, 10, 1);
        var to = new DateTime(2025, 9, 1);

        var result = await pipelineTask.IfDateRangeNotChronological(from, to);

        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Date range is not chronological");
    }

    [Fact]
    public async Task IfDateRangeNotChronological_ShouldNotAddError_WhenRangeIsValid()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var from = new DateTime(2025, 9, 1);
        var to = new DateTime(2025, 10, 1);

        var result = await pipelineTask.IfDateRangeNotChronological(from, to);

        result.Errors.Should().BeEmpty();
    }
}
