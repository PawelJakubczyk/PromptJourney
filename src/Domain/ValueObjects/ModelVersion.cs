using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Validation;

namespace Domain.ValueObjects;

public record ModelVersion : ValueObject<string>, ICreatable<ModelVersion, string>
{
    public const int MaxLength = 10;
    private ModelVersion(string value) : base(value) { }

    public static Result<ModelVersion> Create(string value)
    {
        var result = WorkflowPipeline
            .Empty()
            .Validate(pipeline => pipeline
                .IfNullOrWhitespace<DomainLayer, ModelVersion>(value)
                .IfLengthTooLong<DomainLayer, ModelVersion>(value, MaxLength)
                .IfVersionFormatInvalid<DomainLayer>(value))
            .ExecuteIfNoErrors<ModelVersion>(() => new ModelVersion(value))
            .MapResult(v => v);

        return result;
    }
}

internal static class ModelVersionErrorsExtensions
{
    internal static WorkflowPipeline IfVersionFormatInvalid<TLayer>(
        this WorkflowPipeline pipeline, string value)
        where TLayer : ILayer
    {
        bool isValidNumeric = _validNumericRegex.IsMatch(value);
        bool isValidNiji = _validNijiRegex.IsMatch(value);

        if (!isValidNumeric && !isValidNiji)
        {
            pipeline.Errors.Add(new Error<TLayer>(
                $"Invalid version format: {value}. Expected numeric (e.g., '5', '5.1') or niji format (e.g., 'niji 5')", 
                StatusCodes.Status400BadRequest
            ));
        }

        return pipeline;
    }

    private static readonly Regex _validNumericRegex =
        new(@"^[1-9][0-9]*(\.[0-9])?$", RegexOptions.Compiled);

    private static readonly Regex _validNijiRegex =
        new(@"^niji [1-9][0-9]*$", RegexOptions.Compiled);
}
