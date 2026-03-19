using Application.Abstractions.IRepository;
using Application.Errors;
using Utilities.Errors;
using Utilities.Workflows;

namespace Application.Extensions;

public static class HistoryValidationWorkflowPipelineExtensions
{
    public static async Task<WorkflowPipeline> IfDateInFuture
    (
        this Task<WorkflowPipeline> pipelineTask,
        DateTime date)
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError)
            return pipeline;

        if (date > DateTime.UtcNow)
        {
            errors.Add(ApplicationErrors.HistoryDateInFuture(date));
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> IfDateRangeNotChronological
    (
        this Task<WorkflowPipeline> pipelineTask,
        DateTime from,
        DateTime to
    )
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError)
            return pipeline;

        if (from > to)
        {
            errors.Add(ApplicationErrors.HistoryDateRangeNotChronological(from, to));
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> IfHistoryRecordsLimitNotGreaterThanZero
    (
        this Task<WorkflowPipeline> pipelineTask,
        int count
    )
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError)
            return pipeline;

        if (count <= 0)
        {
            errors.Add(ApplicationErrors.HistoryRequestedCountMustBeGreaterThanZero(count));
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> IfHistoryCountExceedsAvailable
    (
        this Task<WorkflowPipeline> pipelineTask,
        int requestedCount,
        IPromptHistoryRepository repository,
        CancellationToken cancellationToken
    )
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError)
            return pipeline;

        var availableCountResult = await repository.CalculateHistoricalRecordCountAsync(cancellationToken);

        if (availableCountResult.IsFailed)
        {
            errors.AddRange(availableCountResult.Errors.OfType<Error>());
            return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
        }

        if (requestedCount > availableCountResult.Value)
        {
            errors.Add(ApplicationErrors.HistoryRequestedExceedsAvailableRecords(requestedCount, availableCountResult.Value));
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}
