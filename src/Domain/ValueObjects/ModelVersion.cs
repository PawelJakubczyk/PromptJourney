using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using System.Text.RegularExpressions;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record ModelVersion : ValueObject<string>, ICreatable<ModelVersion, string?>
{
    public const int MaxLength = 10;
    private ModelVersion(string value) : base(value) { }

    public static Result<ModelVersion> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<DomainLayer, ModelVersion>(value)
            .Congregate(pipeline => pipeline
                .IfLengthTooLong<DomainLayer, ModelVersion>(value, MaxLength)
                .IfVersionFormatInvalid<DomainLayer>(value))
            .ExecuteIfNoErrors<ModelVersion>(() => new ModelVersion(value))
            .MapResult<ModelVersion>();

        return result;
    }
}

internal static partial class ModelVersionErrorsExtensions
{
    internal const string InvalidVersionFormatMessage =
        $"Invalid version format. Expected numeric (e.g., '5', '5.1') or niji format (e.g., 'niji 5')";

    internal static WorkflowPipeline IfVersionFormatInvalid<TLayer>(
        this WorkflowPipeline pipeline,
        string? value)
        where TLayer : ILayer
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
            pipeline.Errors.Add(
                ErrorFactories.InvalidPattern<string, TLayer>(value, InvalidVersionFormatMessage)
            );
        }

        return pipeline;
    }


    [GeneratedRegex(@"^[1-9][0-9]*(\.[0-9])?$", RegexOptions.Compiled)]
    private static partial Regex ValidNumericRegex();
    [GeneratedRegex(@"^niji [1-9][0-9]*$", RegexOptions.Compiled)]
    private static partial Regex ValidNijiRegex();
}
