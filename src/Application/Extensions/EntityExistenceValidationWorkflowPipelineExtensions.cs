using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using Utilities.Workflows;
using Utilities.Errors;
using Utilities.Results;

namespace Application.Extensions;

public static class EntityExistenceValidationWorkflowPipelineExtensions
{
    public static Task<WorkflowPipeline> IfVersionNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<ModelVersion> version,
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
        Result<ModelVersion> resultVersion,
        IVersionRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfAlreadyExist
        (
            resultVersion,
            repository.CheckVersionExistsAsync,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfParamterAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<Param> parameter,
        IVersionRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfAlreadyExist
        (
            parameter,
            repository.CheckParameterExistsAsync,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfStyleNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<StyleName> style,
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
        Result<StyleName> style,
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
        Result<ExampleLink> link,
        IExampleLinksRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfNotExist
        (
            link,
            (l, ct) => repository.CheckExampleLinkExistsByLinkAsync(l, ct),
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfLinkAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<ExampleLink> link,
        IExampleLinksRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfAlreadyExist
        (
            link,
            (l, ct) => repository.CheckExampleLinkExistsByLinkAsync(l, ct),
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfLinkNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<LinkID> Id,
        IExampleLinksRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfNotExist
        (
            Id,
            (id, ct) => repository.CheckExampleLinkExistsByIdAsync(id, ct),
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfLinkAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<LinkID> id,
        IExampleLinksRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfAlreadyExist
        (
            id,
            (id, ct) => repository.CheckExampleLinkExistsByIdAsync(id, ct),
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfPropertyAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<PropertyName> property,
        Result<ModelVersion> version,
        IPropertiesRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfAlreadyExist
        (
            property,
            (property, cancellationToken) => repository.CheckPropertyExistsInVersionAsync(version.Value, property, cancellationToken),
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfPropertyNotExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<PropertyName> property,
        Result<ModelVersion> version,
        IPropertiesRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.IfNotExist
        (
            property,
            (property, cancellationToken) => repository.CheckPropertyExistsInVersionAsync(version.Value, property, cancellationToken),
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfTagAlreadyExists
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<StyleName> styleName,
        Result<Tag> tag,
        IStyleRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence
        (
            tag,
            (tag, cancellationToken) => repository.CheckTagExistsInStyleAsync(styleName.Value, tag, cancellationToken),
            $"Tag in style '{styleName.Value}'",
            shouldExist: false,
            cancellationToken
        );
    }

    public static Task<WorkflowPipeline> IfTagNotExist
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<StyleName> styleName,
        Result<Tag> tag,
        IStyleRepository repository,
        CancellationToken cancellationToken
    )
    {
        return pipelineTask.ValidateExistence
        (
            tag,
            (tag, cancellationToken) => repository.CheckTagExistsInStyleAsync(styleName.Value, tag, cancellationToken),
            $"Tag in style '{styleName.Value}'",
            shouldExist: true,
            cancellationToken
        );
    }

    public static async Task<WorkflowPipeline> IfAlreadyExist<TType>
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<TType> item,
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
        Result<TType> itemResult,
        Func<TType, CancellationToken, Task<Result<bool>>> existsFunc,
        CancellationToken cancellationToken
    )
    {
        return await ValidateExistence
        (
            pipelineTask,
            itemResult,
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
        Result<TType> itemResult,
        Func<TType, CancellationToken, Task<Result<bool>>> existsFunc,
        string name,
        bool shouldExist,
        CancellationToken cancellationToken
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);

        if (pipeline.BreakOnError || pipeline.Errors.Count > 0 || itemResult.IsFailed)
            return pipeline;

        var item = itemResult.Value;
        var result = await existsFunc(item, cancellationToken).ConfigureAwait(false);
        var errors = pipeline.Errors;

        if (result.IsFailed)
        {
            errors.Add(ErrorFactories.DatabaseConnectionFailed(name));

            return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
        }

        var exists = result.Value;

        if (shouldExist && !exists) errors.Add(ErrorFactories.NotFound<TType>(item));

        if (!shouldExist && exists) errors.Add(ErrorFactories.AlreadyExist<TType>(item));
        
        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}
