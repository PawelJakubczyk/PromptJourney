using Application.Abstractions.IRepository;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Workflows;

namespace Application.Extensions;

public static class HistoryValidationExtensions
{
    internal static string HistoryLimitNotGreaterThanZeroMessage(int count) =>
        $"History count must be greater than zero. Provided: {count}.";
    internal static string HistoryRequestedExceedsAvailableMessage(int requested, int available) =>
        $"Requested {requested} records, but only {available} are available.";

    public static Error HistoryLimitNotGreaterThanZero(int count) =>
        ErrorBuilder.New()
            .WithLayer<ApplicationLayer>()
            .WithMessage(HistoryLimitNotGreaterThanZeroMessage(count))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error HistoryRequestedExceedsAvailable(int requested, int available) =>
        ErrorBuilder.New()
            .WithLayer<ApplicationLayer>()
            .WithMessage(HistoryRequestedExceedsAvailableMessage(requested, available))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();


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
            errors.Add(ErrorFactories.DateInFuture(date));
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
            errors.Add(ErrorFactories.DateRangeNotChronological(from, to));
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> IfHistoryLimitNotGreaterThanZero
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
            errors.Add(HistoryLimitNotGreaterThanZero(count));
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
            errors.Add(HistoryRequestedExceedsAvailable(requestedCount, availableCountResult.Value));
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}
