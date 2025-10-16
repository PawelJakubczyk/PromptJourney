using Application.Abstractions.IRepository;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Extensions;
using Utilities.Workflows;

namespace Application.Extensions;

public static class HistoryValidationExtensions
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
            errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<DomainLayer>()
                .WithMessage($"Date '{date:yyyy-MM-dd}' cannot be in the future.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build()
            );
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
            errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<DomainLayer>()
                .WithMessage($"Date range is not chronological: 'From' ({from:yyyy-MM-dd}) is after 'To' ({to:yyyy-MM-dd}).")
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build()
            );
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
            errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<ApplicationLayer>()
                .WithMessage($"History count must be greater than zero. Provided: {count}.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build()
            );
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
            errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<ApplicationLayer>()
                .WithMessage($"Requested {requestedCount} records, but only {availableCountResult.Value} are available.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build()
            );
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}
