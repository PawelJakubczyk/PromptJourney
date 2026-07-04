using Application.Abstractions.Auth;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.Extensions;

public static class PermisionsValidationWorkflowPipelineExtensions
{
    public static async Task<WorkflowPipeline> IfUserIsNotCurrentUserOrAdmin
    (
        this Task<WorkflowPipeline> pipelineTask,
        ICurrentUser currentUser,
        Result<MidjourneyUser> userIdResult
    )
    {
        var pipeline = await pipelineTask;

        if (pipeline.BreakOnError || pipeline.Errors.Count > 0)
            return pipeline;

        var userId = userIdResult.Value.UserId.ToString();

        if (currentUser.Role != Role.Admin && currentUser.UserId != userId)
        {
            pipeline.Errors.Add(ErrorFactories.Unauthorized(
                "Only admins or the current user can perform this action."));
        }

        return pipeline;
    }
}
