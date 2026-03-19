using Domain.Abstractions;
using Domain.Extensions;
using System.Text.RegularExpressions;
using Utilities.Errors;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.ValueObjects;

public record ModelVersion : ValueObject<string>, ICreatable<ModelVersion, string?>
{
    public const int MaxLength = 10;
    public override bool IsNone => false;
    private ModelVersion(string value) : base(value) { }

    public static Result<ModelVersion> Create(string? value)
    {
        value = value?.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<ModelVersion>(value)
            .CongregateErrors(
                pipeline => pipeline.IfLengthTooLong<ModelVersion>(value!, MaxLength),
                pipeline => pipeline.IfVersionFormatInvalid(value!))
            .ExecuteIfNoErrors<ModelVersion>(() => new ModelVersion(value!))
            .MapResult<ModelVersion>();

        return result;
    }
}

internal static partial class ModelVersionErrorsExtensions
{
    internal const string InvalidVersionFormatMessage =
        $"Invalid version format. Expected numeric (e.g., '5', '5.1') or niji format (e.g., 'niji 5')";

    internal static WorkflowPipeline IfVersionFormatInvalid
    (
        this WorkflowPipeline pipeline,
        string? value
    )
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (value is null)
            return pipeline;

        var isValid =
            ValidNijiRegex().IsMatch(value) ||
            ValidNumericRegex().IsMatch(value);

        if (!isValid)
        {
            pipeline.Errors.Add
            (
                ErrorFactories.InvalidPattern<ModelVersion>(value, InvalidVersionFormatMessage)
            );
        }

        return pipeline;
    }

    [GeneratedRegex(@"^[1-9][0-9]*(\.[0-9])?$", RegexOptions.Compiled)]
    private static partial Regex ValidNumericRegex();
    
    [GeneratedRegex(@"^niji [1-9][0-9]*$", RegexOptions.Compiled)]
    private static partial Regex ValidNijiRegex();
}
