using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Utilities.Constants;
using Utilities.Extensions;
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
            .Validate(pipeline => pipeline
                .IfLengthTooLong<DomainLayer, ModelVersion>(value, MaxLength)
                .IfVersionFormatInvalid<DomainLayer>(value))
            .ExecuteIfNoErrors<ModelVersion>(() => new ModelVersion(value))
            .MapResult<ModelVersion>();

        return result;
    }
}

internal static partial class ModelVersionErrorsExtensions
{
    internal static WorkflowPipeline IfVersionFormatInvalid<TLayer>(
        this WorkflowPipeline pipeline, string? value)
        where TLayer : ILayer
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (value is null) return pipeline;

        if (!ValidNijiRegex().IsMatch(value) && !ValidNumericRegex().IsMatch(value))
        {
            pipeline.Errors.Add
            (
            ErrorFactory.Create()
                .WithLayer<TLayer>()
                .WithMessage($"Invalid version format: {value}. Expected numeric (e.g., '5', '5.1') or niji format (e.g., 'niji 5')")
                .WithErrorCode(StatusCodes.Status400BadRequest)
            );
        }

        return pipeline;
    }

    [GeneratedRegex(@"^[1-9][0-9]*(\.[0-9])?$", RegexOptions.Compiled)]
    private static partial Regex ValidNumericRegex();
    [GeneratedRegex(@"^niji [1-9][0-9]*$", RegexOptions.Compiled)]
    private static partial Regex ValidNijiRegex();
}
