using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record Email : ValueObject<string>, ICreatable<Email, string?>
{
    public const int MaxLength = 320;
    public override bool IsNone => false;

    private Email(string value) : base(value) { }

    public static Result<Email> Create(string? value)
    {
        value = value?.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<Email>(value)
            .AggregateErrors(
                pipeline => pipeline.IfLengthTooLong<Email>(value!, MaxLength),
                pipeline => pipeline.IfEmailFormatInvalid(value!))
            .ExecuteIfNoErrors<Email>(() => new Email(value!))
            .MapResult<Email>();

        return result;
    }
}

internal static partial class EmailErrorsExtensions
{
    private static readonly Regex BasicEmailPattern =
        ValidEmail();

    internal const string InvalidEmailMessage = "Invalid email format.";

    internal static WorkflowPipeline IfEmailFormatInvalid(
        this WorkflowPipeline pipeline,
        string? value)
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (string.IsNullOrWhiteSpace(value))
            return pipeline;

        var isValid =
            BasicEmailPattern.IsMatch(value) &&
            TryMailAddress(value);

        if (!isValid)
        {
            pipeline.Errors.Add(
                ErrorFactories.InvalidPattern<string>(value, InvalidEmailMessage)
            );
        }

        return pipeline;
    }

    private static bool TryMailAddress(string value)
    {
        try
        {
            _ = new MailAddress(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex ValidEmail();
}
