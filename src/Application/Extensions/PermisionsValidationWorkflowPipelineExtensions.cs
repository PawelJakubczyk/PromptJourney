using Application.Abstractions.Auth;
using Domain.ValueObjects;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.Extensions;

public static class PermisionsValidationWorkflowPipelineExtensions
{
    public static async Task<WorkflowPipeline> IfUserIsNotCurrentUserOrAdmin(this Task<WorkflowPipeline> pipelineTask, ICurrentUser _currentUser, Result<UserID> userId)
    {
        var pipeline = await pipelineTask;

        if (pipeline.BreakOnError || pipeline.Errors.Count > 0)
            return pipeline;

        var errors = pipeline.Errors;

        if (_currentUser.Role != Role.Admin && _currentUser.UserId != userId.Value.Value.ToString())
        {
            errors.Add(ErrorFactories.Unauthorized("Only admins or the current user can perform this action."));
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}
