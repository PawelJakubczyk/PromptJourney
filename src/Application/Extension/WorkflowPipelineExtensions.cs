using Application.Abstractions.IRepository;
using Domain.Abstractions;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Extensions;
using Utilities.Workflows;

namespace Application.Extensions;

public static class WorkflowPipelineExtensions
{
    public static Task<WorkflowPipeline> IfVersionNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        ModelVersion version,
        IVersionRepository repo,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence
        (
            version,
            repo.CheckVersionExistsInVersionsAsync,
            "Version",
            shouldExist: true,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfVersionAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        ModelVersion version,
        IVersionRepository repo,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence
        (
            version,
            repo.CheckVersionExistsInVersionsAsync,
            "Version",
            shouldExist: false,
            cancellationToken
        );
    }

    public static async Task<WorkflowPipeline> IfVersionNotInSuportedVersions
    (
        this Task<WorkflowPipeline> pipelineTask,
        ModelVersion version,
        IVersionRepository repo,
        CancellationToken cancellationToken
    )
    {
        var pipeline = await pipelineTask;
        var errors = (await pipelineTask).Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        var result = await repo.GetAllSuportedVersionsAsync(cancellationToken);

        if (result.IsFailed)
        {
            errors.Add
            (
            ErrorFactory.Create()
                .Withlayer(typeof(PersistenceLayer))
                .WithMessage($"Failed to check if {version} is supported version")
                .WithErrorCode(StatusCodes.Status500InternalServerError)
            );
        }

        if (result.IsSuccess && !result.Value.Contains(version))
        {
            errors.Add
            (
            ErrorFactory.Create()
                .Withlayer(typeof(ApplicationLayer))
                .WithMessage($"Version '{version}' is not in supported versions")
                .WithErrorCode(StatusCodes.Status404NotFound)
            );
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static Task<WorkflowPipeline> IfStyleNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        StyleName style,
        IStyleRepository repo,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence
        (
            style,
            repo.CheckStyleExistsAsync,
            "Style",
            shouldExist: true,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfStyleAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        StyleName style,
        IStyleRepository repo,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence
        (
            style,
            repo.CheckStyleExistsAsync,
            "Style",
            shouldExist: false,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfLinkNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        ExampleLink link,
        IExampleLinksRepository repo,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence(
            link,
            repo.CheckExampleLinkExistsAsync,
            "Link",
            shouldExist: true,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfPropertyAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        PropertyName property,
        ModelVersion version,
        IPropertiesRepository repo,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence(
            property,
            (property, ct) => repo.CheckParameterExistsInVersionAsync(version, property, ct),
            "Property",
            shouldExist: false,
            cancellationToken
        );
    }
    public static Task<WorkflowPipeline> IfPropertyNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        PropertyName property,
        ModelVersion version,
        IPropertiesRepository repo,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence(
            property,
            (property, ct) => repo.CheckParameterExistsInVersionAsync(version, property, ct),
            "Property",
            shouldExist: true,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfLinkAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        ExampleLink link,
        IExampleLinksRepository repo,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence(
            link,
            repo.CheckExampleLinkExistsAsync,
            "Link",
            shouldExist: false,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfTagAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        StyleName styleName,
        Tag tag,
        IStyleRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence(
            tag,
            (t, ct) => repository.CheckTagExistsInStyleAsync(styleName, t, ct),
            $"Tag in style '{styleName}'",
            shouldExist: false,
            cancellationToken
        );
    }


    public static Task<WorkflowPipeline> IfTagNotExist
    (
        this Task<WorkflowPipeline> pipelineTask,
        StyleName styleName,
        Tag tag,
        IStyleRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence(
            tag,
            (t, ct) => repository.CheckTagExistsInStyleAsync(styleName, t, ct),
            $"Tag in style '{styleName}'",
            shouldExist: true,
            cancellationToken
        );
    }

    public static async Task<WorkflowPipeline> IfDateInFuture
    (
        this Task<WorkflowPipeline> pipelineTask,
        DateTime date)
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        if (date > DateTime.UtcNow)
        {
            errors.Add
            (
            ErrorFactory.Create()
                .Withlayer(typeof(DomainLayer))
                .WithMessage($"Date '{date:yyyy-MM-dd}' cannot be in the future.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
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

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        if (from > to)
        {
            errors.Add
            (
            ErrorFactory.Create()
                .Withlayer(typeof(DomainLayer))
                .WithMessage($"Date range is not chronological: 'From' ({from:yyyy-MM-dd}) is after 'To' ({to:yyyy-MM-dd}).")
                .WithErrorCode(StatusCodes.Status400BadRequest)
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

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        if (count <= 0)
        {
            errors.Add
            (
            ErrorFactory.Create()
                .Withlayer(typeof(ApplicationLayer))
                .WithMessage($"History count must be greater than zero. Provided: {count}.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
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

        if (pipeline.BreakOnError && errors.Count != 0)
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
            ErrorFactory.Create()
                .Withlayer(typeof(ApplicationLayer))
                .WithMessage($"Requested {requestedCount} records, but only {availableCountResult.Value} are available.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
            );
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> IfListIsNullOrEmpty<TValue>(
        this Task<WorkflowPipeline> pipelineTask,
        List<TValue>? items)
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        if (items is null || items.Count == 0)
        {
            var name = typeof(TValue).Name;
            errors.Add
            (
            ErrorFactory.Create()
                .Withlayer(typeof(ApplicationLayer))
                .WithMessage($"List of '{name}' must not be empty.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
            );
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> ValidateExistence<TType>
    (
        this Task<WorkflowPipeline> pipelineTask,
        TType item,
        Func<TType, CancellationToken, Task<Result<bool>>> existsFunc,
        string entityName,
        bool shouldExist,
        CancellationToken cancellationToken
    )
        where TType : ValueObject<string>
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        var result = await existsFunc(item, cancellationToken).ConfigureAwait(false);

        if (result.IsFailed)
        {
            errors.Add
            (
            ErrorFactory.Create()
                .Withlayer(typeof(PersistenceLayer))
                .WithMessage($"Failed to check if {entityName} exists")
                .WithErrorCode(StatusCodes.Status404NotFound)
            );
        }

        if (result.IsSuccess && result.Value != shouldExist)
        {
            errors.Add
            (
            ErrorFactory.Create()
                .Withlayer(typeof(ApplicationLayer))
                .WithMessage($"{entityName} '{item}' {(shouldExist ? "not found" : "already exists")}")
                .WithErrorCode(StatusCodes.Status409Conflict)
            );
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}