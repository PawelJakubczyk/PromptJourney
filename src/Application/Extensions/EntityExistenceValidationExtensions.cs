using Application.Abstractions.IRepository;
using Domain.Abstractions;
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
    IVersionRepository repo,
    CancellationToken cancellationToken
)
    {
        return pipelineTask.ValidateExistence
        (
            version,
            repo.CheckVersionExists,
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
            repo.CheckVersionExists,
            "Version",
            shouldExist: false,
            cancellationToken
        );
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
            (property, ct) => repo.CheckPropertyExistsInVersionAsync(version, property, ct),
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
            (property, ct) => repo.CheckPropertyExistsInVersionAsync(version, property, ct),
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
