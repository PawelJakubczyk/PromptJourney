using Application.Abstractions.IRepository;
using Domain.Abstractions;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Validation;

namespace Application.Extension;

public static class ValidationPipelineExtensions
{
    public static Task<ValidationPipeline> IfVersionNotExists
    (
        this Task<ValidationPipeline> pipelineTask,
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

    public static Task<ValidationPipeline> IfVersionAlreadyExists
    (
        this Task<ValidationPipeline> pipelineTask,
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

    public static Task<ValidationPipeline> IfStyleNotExists
    (
        this Task<ValidationPipeline> pipelineTask,
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

    public static Task<ValidationPipeline> IfStyleAlreadyExists
    (
        this Task<ValidationPipeline> pipelineTask,
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

    public static Task<ValidationPipeline> IfLinkNotExists
    (
        this Task<ValidationPipeline> pipelineTask,
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

    public static Task<ValidationPipeline> IfLinkAlreadyExists
    (
        this Task<ValidationPipeline> pipelineTask,
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

    public static async Task<ValidationPipeline> IfTagAlreadyExists
    (
        this Task<ValidationPipeline> pipelineTask,
        StyleName styleName,
        Tag tag,
        IStyleRepository repository,
        CancellationToken cancellationToken
    )
    {
        var pipeline = await pipelineTask;
        var errors =  pipeline.Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        var result = await repository.CheckTagExistsInStyleAsync(styleName, tag, cancellationToken);
        if (result.IsFailed)
        {
            errors.AddRange(result.Errors.OfType<Error>());
        }
        else if (result.Value)
        {
            errors.Add(new Error<ApplicationLayer>($"Tag '{tag}' already exists in style '{styleName}'."));
        }

        return ValidationPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<ValidationPipeline> IfTagNotExist
    (
        this Task<ValidationPipeline> pipelineTask,
        StyleName styleName,
        Tag tag,
        IStyleRepository repository,
        CancellationToken cancellationToken
    )
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        var result = await repository.CheckTagExistsInStyleAsync(styleName, tag, cancellationToken);
        if (result.IsFailed)
        {
            errors.AddRange(result.Errors.OfType<Error>());
        }
        else if (!result.Value)
        {
            errors.Add(new Error<ApplicationLayer>($"Tag '{tag}' does not exist in style '{styleName}'."));
        }

        return ValidationPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<ValidationPipeline> IfDateInFuture
    (
        this Task<ValidationPipeline> pipelineTask,
        DateTime date)
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        if (date > DateTime.UtcNow)
        {
            errors.Add(new Error<DomainLayer>($"Date '{date:yyyy-MM-dd}' cannot be in the future."));
        }

        return ValidationPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<ValidationPipeline> IfDateRangeNotChronological
    (
        this Task<ValidationPipeline> pipelineTask,
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
            errors.Add(new Error<DomainLayer>(
                $"Date range is not chronological: 'From' ({from:yyyy-MM-dd}) is after 'To' ({to:yyyy-MM-dd})."));
        }

        return ValidationPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<ValidationPipeline> IfHistoryLimitNotGreaterThanZero
    (
        this Task<ValidationPipeline> pipelineTask,
        int count
    )
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        if (count <= 0)
        {
            errors.Add(new Error($"History count must be greater than zero. Provided: {count}."));
        }

        return ValidationPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<ValidationPipeline> IfHistoryCountExceedsAvailable
    (
        this Task<ValidationPipeline> pipelineTask,
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
            return ValidationPipeline.Create(errors, pipeline.BreakOnError);
        }

        if (requestedCount > availableCountResult.Value)
        {
            errors.Add(new Error(
                $"Requested {requestedCount} records, but only {availableCountResult.Value} are available."));
        }

        return ValidationPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<ValidationPipeline> IfListIsNullOrEmpty<TValue>(
        this Task<ValidationPipeline> pipelineTask,
        List<TValue>? items)
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        if (items is null || items.Count == 0)
        {
            var name = typeof(TValue).Name;
            errors.Add(new Error($"List of '{name}' must not be empty."));
        }

        return ValidationPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<ValidationPipeline> ValidateExistence<TType>
    (
        this Task<ValidationPipeline> pipelineTask,
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
            errors.Add(new Error<PersistenceLayer>($"Failed to check if {entityName} exists"));
        }

        if (result.IsSuccess && result.Value != shouldExist)
        {
            errors.Add(new Error<ApplicationLayer>(
                $"{entityName} '{item}' {(shouldExist ? "not found" : "already exists")}"
            ));
        }

        return ValidationPipeline.Create(errors, pipeline.BreakOnError);
    }
}