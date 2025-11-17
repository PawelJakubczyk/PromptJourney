using Domain.Abstractions;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Workflows;

namespace Domain.Extensions;

public static class ListValidationExtension
{
    public static async Task<WorkflowPipeline> IfListIsNullOrEmpty<TValue>
    (
        this Task<WorkflowPipeline> pipelineTask,
        List<TValue>? items
    )
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError)
            return pipeline;

        if (items is null || items.Count == 0)
        {
            errors.Add(ErrorFactories.NullOrEmpty<TValue, ApplicationLayer>());
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static WorkflowPipeline IfListIsNull<TLayer, TValue>
    (
        this WorkflowPipeline pipeline,
        List<TValue?>? value
    )
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (value is null)
        {
            pipeline.Errors.Add(
                ErrorFactories.Null<TValue, TLayer>()
            );
        }
        return pipeline;
    }

    public static WorkflowPipeline IfListIsEmpty<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        List<TValue>? items)
        where TLayer : ILayer
        where TValue : ValueObject<string?>?
    {
        if (items != null && items.Count == 0)
        {
            pipeline.Errors.Add(
                ErrorFactories.Empty<TValue, TLayer>()
            );
        }
        return pipeline;
    }
    public static WorkflowPipeline IfListNotContain<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        List<TValue>? items,
        TValue element)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (items != null && !items.Contains(element))
        {
            pipeline.Errors.Add(
                ErrorFactories.CollectionNotContain<TValue, TLayer>(items)
            );
        }
        return pipeline;
    }

    public static WorkflowPipeline IfListContain<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        List<TValue>? items,
        TValue element)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (items != null && items.Contains(element))
        {
            pipeline.Errors.Add(
                ErrorFactories.CollectionAlreadyContains<TValue, TLayer>(element)
            );
        }
        return pipeline;
    }
}