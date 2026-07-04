using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record UserName : ValueObject<string>, ICreatable<UserName, string?>
{
    public const int MaxLength = 100;
    public override bool IsNone => false;

    private UserName(string value) : base(value) { }

    public static Result<UserName> Create(string? value)
    {
        value = value?.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<UserName>(value)
            .AggregateErrors(
                pipeline => pipeline.IfLengthTooLong<UserName>(value!, MaxLength),
                pipeline => pipeline.IfUserNameFormatInvalid(value!))
            .ExecuteIfNoErrors<UserName>(() => new UserName(value!))
            .MapResult<UserName>();

        return result;
    }
}

internal static partial class UserNameErrorsExtensions
{
    private static readonly Regex Pattern = ValidUserName();

    internal const string InvalidUserNameMessage =
        "Username may contain only letters, digits, spaces, underscores and hyphens.";

    internal static WorkflowPipeline IfUserNameFormatInvalid(
        this WorkflowPipeline pipeline,
        string? value)
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (string.IsNullOrWhiteSpace(value))
            return pipeline;

        if (!Pattern.IsMatch(value))
        {
            pipeline.Errors.Add(
                ErrorFactories.InvalidPattern<string>(value, InvalidUserNameMessage)
            );
        }

        return pipeline;
    }

    [GeneratedRegex("^[a-zA-Z0-9 _-]+$", RegexOptions.Compiled)]
    private static partial Regex ValidUserName();
}
