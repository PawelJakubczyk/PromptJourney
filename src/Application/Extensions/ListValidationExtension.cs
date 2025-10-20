using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Extensions;
using Utilities.Workflows;

namespace Application.Extensions;

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
            var name = typeof(TValue).Name;
            errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<ApplicationLayer>()
                .WithMessage($"List of '{name}' must not be empty.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build()
            );
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}