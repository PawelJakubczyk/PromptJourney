using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record Role : ValueObject<string>, ICreatable<Role, string?>
{
    public const int MaxLength = 10;
    public override bool IsNone => false;

    internal static readonly HashSet<string> AllowedRoles =
    [
        "User",
        "Admin"
    ];

    private Role(string value) : base(value) { }

    public static Result<Role> Create(string? value)
    {
        value = value?.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<Role>(value)
            .IfRoleInvalid(value!)
            .ExecuteIfNoErrors<Role>(() => new Role(value!))
            .MapResult<Role>();

        return result;
    }
}

internal static class RoleErrorsExtensions
{
    internal const string InvalidRoleMessage = "Invalid role value.";

    internal static WorkflowPipeline IfRoleInvalid
    (
        this WorkflowPipeline pipeline,
        string value
    )
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (!Role.AllowedRoles.Contains(value))
        {
            pipeline.Errors.Add(
                ErrorFactories.InvalidPattern<string>(value, InvalidRoleMessage)
            );
        }

        return pipeline;
    }
}
