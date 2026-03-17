using Utilities.Errors;
using Utilities.Workflows;

namespace Application.Extensions;

public static class ListValidationWorkflowPipelineExtensions
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
            errors.Add(ErrorFactories.NullOrEmpty<TValue>());
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}
