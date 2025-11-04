using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Extensions;
using Utilities.Workflows;

namespace Application.Extensions;

public static class EntityExistenceValidationExtensions
{
    public static Task<WorkflowPipeline> IfVersionNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        ModelVersion version,
        IVersionRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfNotExist
        (
            version,
            repository.CheckVersionExistsAsync,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfVersionAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        ModelVersion version,
        IVersionRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfAlreadyExist
        (
            version,
            repository.CheckVersionExistsAsync,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfStyleNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        StyleName style,
        IStyleRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfNotExist
        (
            style,
            repository.CheckStyleExistsAsync,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfStyleAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        StyleName style,
        IStyleRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfAlreadyExist
        (
            style,
            repository.CheckStyleExistsAsync,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfLinkNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        ExampleLink link,
        IExampleLinksRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfNotExist
        (
            link,
            repository.CheckExampleLinkExistsByLinkAsync,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfLinkNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        Guid Id,
        IExampleLinksRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfNotExist
        (
            Id,
            repository.CheckExampleLinkExistsByIdAsync,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfLinkAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        ExampleLink link,
        IExampleLinksRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfAlreadyExist(
            link,
            repository.CheckExampleLinkExistsByLinkAsync,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfPropertyAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        PropertyName property,
        ModelVersion version,
        IPropertiesRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfAlreadyExist(
            property,
            (property, cancellationToken) => repository.CheckPropertyExistsInVersionAsync(version, property, cancellationToken),
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfPropertyNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        PropertyName property,
        ModelVersion version,
        IPropertiesRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfNotExist(
            property,
            (property, cancellationToken) => repository.CheckPropertyExistsInVersionAsync(version, property, cancellationToken),
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
            (tag, cancellationToken) => repository.CheckTagExistsInStyleAsync(styleName, tag, cancellationToken),
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
            (tag, cancellationToken) => repository.CheckTagExistsInStyleAsync(styleName, tag, cancellationToken),
            $"Tag in style '{styleName}'",
            shouldExist: true,
            cancellationToken
        );
    }

    public static async Task<WorkflowPipeline> IfAlreadyExist<TType>
    (
        this Task<WorkflowPipeline> pipelineTask,
        TType item,
        Func<TType, CancellationToken, Task<Result<bool>>> existsFunc,
        CancellationToken cancellationToken
    )
    {
        return await ValidateExistence
        (
            pipelineTask,
            item,
            existsFunc,
            typeof(TType).Name,
            shouldExist: false,
            cancellationToken
        )
        .ConfigureAwait(false);
    }

    public static async Task<WorkflowPipeline> IfNotExist<TType>
    (
        this Task<WorkflowPipeline> pipelineTask,
        TType item,
        Func<TType, CancellationToken, Task<Result<bool>>> existsFunc,
        CancellationToken cancellationToken
    )
    {
        return await ValidateExistence
        (
            pipelineTask,
            item,
            existsFunc,
            typeof(TType).Name,
            shouldExist: true,
            cancellationToken
        )
        .ConfigureAwait(false);
    }

    public static async Task<WorkflowPipeline> ValidateExistence<TType>
    (
        this Task<WorkflowPipeline> pipelineTask,
        TType item,
        Func<TType, CancellationToken, Task<Result<bool>>> existsFunc,
        string Name,
        bool shouldExist,
        CancellationToken cancellationToken
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError)
            return pipeline;

        var result = await existsFunc(item, cancellationToken).ConfigureAwait(false);

        if (result.IsFailed)
        {
            errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<PersistenceLayer>()
                .WithMessage($"Failed to check if {Name} exists")
                .WithErrorCode(StatusCodes.Status404NotFound)
                .Build()
            );
        }

        var state = shouldExist ? "not found" : "already exists";

        if (result.IsSuccess && result.Value != shouldExist)
        {
            errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<ApplicationLayer>()
                .WithMessage($"{Name} '{item}' {state}")
                .WithErrorCode(StatusCodes.Status409Conflict)
                .Build()
            );
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}
